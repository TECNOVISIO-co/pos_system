#!/usr/bin/env python3
import json
import re
import subprocess
from dataclasses import dataclass
from pathlib import Path
from typing import List, Optional

MDB_PATH = Path(__file__).resolve().parents[1] / "bd1.mdb"
OUTPUT_PATH = Path(__file__).resolve().parents[1] / "docs" / "01_data_dictionary.md"

# Focus on the legacy tables that feed the modern model
TABLES = [
    "productos",
    "clientes",
    "terceros",
    "facturas",
    "detalle_factura",
    "impuestos",
    "listas_precios_productos",
    "detalle_pago",
    "cuentas_por_cobrar_gastos",
    "detalle_cuentas_por_cobrar_gastos",
    "bodegas",
    "detalle_producto_bodega",
    "usuarios",
    "vendedores",
    "detalle_usuario_permiso",
    "movimientos",
]

COLUMN_RE = re.compile(
    r"^\s*\[(?P<name>[^\]]+)\]\s+(?P<type>[^,]+?)(?:,)?\s*$",
    re.IGNORECASE,
)

@dataclass
class Column:
    name: str
    raw_type: str
    oledb_type: str
    length: Optional[int]
    nullable: bool

    @classmethod
    def from_schema_line(cls, line: str) -> Optional["Column"]:
        match = COLUMN_RE.match(line)
        if not match:
            return None
        raw = match.group("type").strip()
        nullable = "NOT NULL" not in raw.upper()
        cleaned = raw.replace("NOT NULL", "").strip()
        width = None
        width_match = re.search(r"\((\d+)\)", cleaned)
        if width_match:
            width = int(width_match.group(1))
        oledb_type = cleaned.strip()
        return cls(
            name=match.group("name").strip(),
            raw_type=raw,
            oledb_type=oledb_type,
            length=width,
            nullable=nullable,
        )


def run_mdb_schema(table: str) -> str:
    result = subprocess.run(
        ["mdb-schema", "-T", table, str(MDB_PATH)],
        check=True,
        capture_output=True,
        text=True,
    )
    return result.stdout


def parse_table(table: str) -> List[Column]:
    schema = run_mdb_schema(table)
    columns: List[Column] = []
    inside_table = False
    for line in schema.splitlines():
        if line.strip().upper().startswith("CREATE TABLE"):
            inside_table = True
            continue
        if not inside_table:
            continue
        if line.strip().startswith(");"):
            break
        column = Column.from_schema_line(line)
        if column:
            columns.append(column)
    return columns


def detect_natural_keys(columns: List[Column]) -> List[str]:
    candidates: List[str] = []
    for column in columns:
        lowered = column.name.lower()
        if any(keyword in lowered for keyword in ("codigo", "numero", "documento")):
            candidates.append(column.name)
        if lowered in ("nit", "email", "identificacion"):
            candidates.append(column.name)
    return sorted(dict.fromkeys(candidates))


def create_markdown_table(columns: List[Column]) -> str:
    header = "| Columna | Tipo OLEDB | Longitud | Permite nulos |\n| --- | --- | --- | --- |"
    rows = []
    for column in columns:
        length = column.length if column.length is not None else "-"
        nullable = "Sí" if column.nullable else "No"
        rows.append(f"| `{column.name}` | {column.oledb_type} | {length} | {nullable} |")
    return "\n".join([header, *rows])


def main() -> None:
    if not MDB_PATH.exists():
        raise SystemExit(f"No se encontró la base de datos Access en {MDB_PATH}")

    OUTPUT_PATH.parent.mkdir(parents=True, exist_ok=True)

    content = [
        "# Diccionario de Datos (Access Origen)",
        "",
        "Este documento se genera automáticamente a partir del archivo `bd1.mdb` mediante `mdb-schema`.",
        "",
        "```json",
        json.dumps({"tables": TABLES}, ensure_ascii=False, indent=2),
        "```",
        "",
        "---",
    ]

    for table in TABLES:
        try:
            columns = parse_table(table)
        except subprocess.CalledProcessError as exc:
            content.extend([
                f"\n## {table}",
                "",
                f"> Error al leer la tabla: {exc}",
            ])
            continue

        content.extend([f"\n## {table}", ""])
        if not columns:
            content.append("> No se detectaron columnas (verificar permisos o codificación).")
            continue
        content.append(create_markdown_table(columns))
        natural_keys = detect_natural_keys(columns)
        if natural_keys:
            content.extend([
                "",
                "**Posibles claves naturales:** " + ", ".join(f"`{key}`" for key in natural_keys),
            ])

    OUTPUT_PATH.write_text("\n".join(content), encoding="utf-8")
    print(f"Diccionario generado en {OUTPUT_PATH}")


if __name__ == "__main__":
    main()
