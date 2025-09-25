# Diccionario de Datos (Access Origen)

Este documento se genera automáticamente a partir del archivo `bd1.mdb` mediante `mdb-schema`.

```json
{
  "tables": [
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
    "movimientos"
  ]
}
```

---

## productos

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `consecutivo` | Long Integer | - | Sí |
| `codigo` | Text (50) | 50 | No |
| `marca` | Text (255) | 255 | Sí |
| `modelo` | Text (255) | 255 | Sí |
| `precio_compra` | Double | - | Sí |
| `precio_venta` | Double | - | Sí |
| `precio_venta_2` | Double | - | Sí |
| `linea` | Text (255) | 255 | Sí |
| `iva` | Double | - | Sí |
| `retirado` | Text (1) | 1 | Sí |
| `retirado_por_usuario` | Text (50) | 50 | Sí |
| `motivo_retiro` | Text (255) | 255 | Sí |
| `existencias` | Double | - | Sí |
| `cantidad_minima` | Double | - | Sí |
| `imagen` | Text (255) | 255 | Sí |
| `producto_elaborado` | Text (1) | 1 | Sí |
| `unidad_de_medida` | Text (20) | 20 | Sí |
| `registro_en _uso` | Text (255) | 255 | Sí |
| `serial` | Text (255) | 255 | Sí |
| `lote` | Text (255) | 255 | Sí |
| `cantidad_para_promocion` | Double | - | Sí |
| `descuento` | Double | - | Sí |
| `descuento_maximo` | Double | - | Sí |
| `unidad_de_medida_2` | Text (20) | 20 | Sí |
| `existencias_2` | Double | - | Sí |
| `codigo_referencia` | Text (50) | 50 | Sí |
| `impuesto2` | Double | - | Sí |
| `impuesto3` | Double | - | Sí |
| `codigo_impuesto1` | Double | - | Sí |
| `codigo_impuesto2` | Double | - | Sí |
| `codigo_impuesto3` | Double | - | Sí |
| `base_no_gravada` | Double | - | Sí |
| `forzar_validacion_de_inventario` | Text (1) | 1 | Sí |
| `discriminar_lista_de_materia_prima_en_ventas` | Text (1) | 1 | Sí |
| `discriminar_impuestos_de_materia_prima_en_ventas` | Text (1) | 1 | Sí |
| `observaciones` | Memo/Hyperlink (255) | 255 | Sí |
| `ultimo_costo` | Double | - | Sí |
| `porcentaje_utilidad_sugerida` | Double | - | Sí |
| `descripcion` | Text (255) | 255 | Sí |

**Posibles claves naturales:** `codigo`, `codigo_impuesto1`, `codigo_impuesto2`, `codigo_impuesto3`, `codigo_referencia`

## clientes

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `consecutivo` | Long Integer | - | Sí |
| `codigo` | Text (20) | 20 | No |
| `nit` | Text (15) | 15 | Sí |
| `nombre` | Text (50) | 50 | Sí |
| `direccion` | Text (50) | 50 | Sí |
| `ciudad` | Text (50) | 50 | Sí |
| `telefono` | Text (50) | 50 | Sí |
| `celular` | Text (50) | 50 | Sí |
| `e_mail` | Text (100) | 100 | Sí |
| `asesor` | Text (50) | 50 | Sí |
| `observaciones` | Text (255) | 255 | Sí |
| `contacto_directo` | Text (50) | 50 | Sí |
| `Retirado` | Text (1) | 1 | Sí |
| `retirado_por_usuario` | Text (50) | 50 | Sí |
| `motivo_retiro` | Text (255) | 255 | Sí |
| `porcentaje_descuento` | Double | - | Sí |
| `cupo_maximo_credito` | Double | - | Sí |
| `reteica` | Double | - | Sí |
| `retefuente` | Double | - | Sí |
| `propiedad_retencion` | Text (255) | 255 | Sí |
| `saldo_puntos_acumulados` | Double | - | Sí |
| `fecha_nacimiento` | DateTime | - | Sí |
| `lista_precios_productos` | Text (50) | 50 | Sí |
| `tipo_documento` | Text (50) | 50 | Sí |
| `tipo_contribuyente` | Text (50) | 50 | Sí |
| `regimen_contable` | Text (255) | 255 | Sí |
| `departamento` | Text (20) | 20 | Sí |
| `pais` | Text (20) | 20 | Sí |
| `razon_social` | Text (255) | 255 | Sí |
| `codigo_municipio` | Text (20) | 20 | Sí |
| `actividad_economica` | Text (50) | 50 | Sí |
| `barrio` | Text (255) | 255 | Sí |
| `zona` | Text (50) | 50 | Sí |

**Posibles claves naturales:** `codigo`, `codigo_municipio`, `nit`, `tipo_documento`

## terceros

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `consecutivo` | Long Integer | - | Sí |
| `codigo` | Text (20) | 20 | No |
| `nit` | Text (15) | 15 | Sí |
| `nombre` | Text (50) | 50 | Sí |
| `direccion` | Text (50) | 50 | Sí |
| `ciudad` | Text (50) | 50 | Sí |
| `telefono` | Text (50) | 50 | Sí |
| `celular` | Text (50) | 50 | Sí |
| `e_mail` | Text (100) | 100 | Sí |
| `asesor` | Text (50) | 50 | Sí |
| `observaciones` | Text (255) | 255 | Sí |
| `contacto_directo` | Text (50) | 50 | Sí |
| `Retirado` | Text (1) | 1 | Sí |
| `retirado_por_usuario` | Text (50) | 50 | Sí |
| `motivo_retiro` | Text (255) | 255 | Sí |
| `porcentaje_descuento` | Double | - | Sí |
| `cupo_maximo_credito` | Double | - | Sí |
| `reteica` | Double | - | Sí |
| `retefuente` | Double | - | Sí |
| `propiedad_retencion` | Text (255) | 255 | Sí |
| `saldo_puntos_acumulados` | Double | - | Sí |
| `fecha_nacimiento` | DateTime | - | Sí |
| `lista_precios_productos` | Text (50) | 50 | Sí |
| `tipo_documento` | Text (50) | 50 | Sí |
| `tipo_contribuyente` | Text (50) | 50 | Sí |
| `regimen_contable` | Text (255) | 255 | Sí |
| `departamento` | Text (255) | 255 | Sí |
| `pais` | Text (50) | 50 | Sí |
| `razon_social` | Text (255) | 255 | Sí |
| `codigo_municipio` | Text (20) | 20 | Sí |
| `actividad_economica` | Text (50) | 50 | Sí |
| `barrio` | Text (255) | 255 | Sí |
| `zona` | Text (50) | 50 | Sí |

**Posibles claves naturales:** `codigo`, `codigo_municipio`, `nit`, `tipo_documento`

## facturas

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `numero` | Long Integer | - | Sí |
| `codigo_cliente` | Text (20) | 20 | Sí |
| `atencion_a` | Text (50) | 50 | Sí |
| `fecha` | DateTime | - | Sí |
| `vigencia` | Text (20) | 20 | Sí |
| `condiciones_pago` | Text (50) | 50 | Sí |
| `numero_solicitud` | Long Integer | - | Sí |
| `observaciones` | Text (100) | 100 | Sí |
| `elaborada` | Text (20) | 20 | Sí |
| `aprobada` | Text (20) | 20 | Sí |
| `anulada` | Text (1) | 1 | Sí |
| `anulada_por_usuario` | Text (20) | 20 | Sí |
| `motivo_anulacion` | Text (100) | 100 | Sí |
| `fecha_anulacion` | DateTime | - | Sí |
| `cancelada` | Text (1) | 1 | Sí |
| `descuento_condicionado` | Double | - | Sí |
| `nombre_vendedor` | Text (50) | 50 | Sí |
| `atendido` | Text (1) | 1 | Sí |
| `id_mesa` | Long Integer | - | Sí |
| `fecha_apertura` | DateTime | - | Sí |
| `atendido2` | Text (1) | 1 | Sí |
| `atendido3` | Text (1) | 1 | Sí |
| `atendido4` | Text (1) | 1 | Sí |
| `fecha_atendido` | DateTime | - | Sí |
| `fecha_atendido2` | DateTime | - | Sí |
| `fecha_atendido3` | DateTime | - | Sí |
| `fecha_atendido4` | DateTime | - | Sí |
| `factura_electronica_aprobada` | Text (1) | 1 | Sí |
| `factura_electronica_codigo_mensaje_recibido` | Text (255) | 255 | Sí |
| `factura_electronica_mensaje_recibido` | Text (255) | 255 | Sí |
| `referencia_1` | Text (50) | 50 | Sí |
| `referencia_2` | Text (50) | 50 | Sí |
| `referencia_3` | Text (50) | 50 | Sí |
| `paga_con` | Double | - | Sí |

**Posibles claves naturales:** `codigo_cliente`, `factura_electronica_codigo_mensaje_recibido`, `numero`, `numero_solicitud`

## detalle_factura

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `item` | Long Integer | - | Sí |
| `num_factura` | Long Integer | - | Sí |
| `cod_producto` | Text (50) | 50 | Sí |
| `id_tercero` | Text (20) | 20 | Sí |
| `entrega` | Text (255) | 255 | Sí |
| `cantidad` | Double | - | Sí |
| `valor_unitario` | Double | - | Sí |
| `impuesto` | Double | - | Sí |
| `descuento` | Double | - | Sí |
| `costo_promedio` | Double | - | Sí |
| `id_vendedor` | Text (20) | 20 | Sí |
| `comision` | Double | - | Sí |
| `peso_recipiente1` | Double | - | Sí |
| `numero_recipientes1` | Double | - | Sí |
| `peso_recipiente2` | Double | - | Sí |
| `numero_recipientes2` | Double | - | Sí |
| `peso_recipiente3` | Double | - | Sí |
| `numero_recipientes3` | Double | - | Sí |
| `impuesto2` | Double | - | Sí |
| `impuesto3` | Double | - | Sí |
| `codigo_impuesto1` | Double | - | Sí |
| `codigo_impuesto2` | Double | - | Sí |
| `codigo_impuesto3` | Double | - | Sí |
| `base_no_gravada` | Double | - | Sí |
| `seriales` | Memo/Hyperlink (255) | 255 | Sí |
| `id_promocion_aplicada` | Long Integer | - | Sí |
| `id_bodega_inventario` | Text (20) | 20 | Sí |

**Posibles claves naturales:** `codigo_impuesto1`, `codigo_impuesto2`, `codigo_impuesto3`, `numero_recipientes1`, `numero_recipientes2`, `numero_recipientes3`

## impuestos

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `id_impuesto` | Double | - | No |
| `nombre_impuesto` | Text (255) | 255 | Sí |
| `porcentaje_impuesto` | Double | - | Sí |
| `tipo_calculo_de_impuesto` | Text (255) | 255 | Sí |
| `deshabilitado` | Text (1) | 1 | Sí |

## listas_precios_productos

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `nombre_lista_precios` | Text (50) | 50 | No |
| `codigo_producto` | Text (50) | 50 | No |
| `precio_venta` | Double | - | Sí |

**Posibles claves naturales:** `codigo_producto`

## detalle_pago

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `numero_documento` | Long Integer | - | No |
| `tipo_documento` | Text (50) | 50 | No |
| `id_tercero` | Text (255) | 255 | No |
| `total_efectivo` | Double | - | Sí |
| `total_tarjeta_credito` | Double | - | Sí |
| `total_tarjeta_debito` | Double | - | Sí |
| `total_cheque` | Double | - | Sí |
| `total_bonos` | Double | - | Sí |
| `total_dinero_redimido_de_puntos` | Double | - | Sí |
| `observaciones` | Text (255) | 255 | Sí |
| `total_tarjeta_codensa` | Double | - | Sí |
| `total_bonos_sodexo` | Double | - | Sí |
| `total_medio_pago_personalizado_1` | Double | - | Sí |
| `total_medio_pago_personalizado_2` | Double | - | Sí |
| `total_medio_pago_personalizado_3` | Double | - | Sí |
| `total_medio_pago_personalizado_4` | Double | - | Sí |
| `total_medio_pago_personalizado_5` | Double | - | Sí |
| `total_medio_pago_personalizado_6` | Double | - | Sí |
| `total_medio_pago_personalizado_7` | Double | - | Sí |
| `total_medio_pago_personalizado_8` | Double | - | Sí |
| `total_medio_pago_personalizado_9` | Double | - | Sí |
| `total_medio_pago_personalizado_10` | Double | - | Sí |

**Posibles claves naturales:** `numero_documento`, `tipo_documento`

## cuentas_por_cobrar_gastos

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `numero` | Long Integer | - | Sí |
| `codigo_cliente` | Text (20) | 20 | Sí |
| `atencion_a` | Text (50) | 50 | Sí |
| `fecha` | DateTime | - | Sí |
| `vigencia` | Text (20) | 20 | Sí |
| `condiciones_pago` | Text (50) | 50 | Sí |
| `numero_solicitud` | Long Integer | - | Sí |
| `observaciones` | Text (100) | 100 | Sí |
| `elaborada` | Text (20) | 20 | Sí |
| `aprobada` | Text (20) | 20 | Sí |
| `anulada` | Text (1) | 1 | Sí |
| `anulada_por_usuario` | Text (20) | 20 | Sí |
| `motivo_anulacion` | Text (100) | 100 | Sí |
| `fecha_anulacion` | DateTime | - | Sí |
| `cancelada` | Text (1) | 1 | Sí |
| `paga_con` | Long Integer | - | Sí |
| `descuento_condicionado` | Double | - | Sí |
| `nombre_vendedor` | Text (50) | 50 | Sí |
| `atendido` | Text (1) | 1 | Sí |
| `id_mesa` | Long Integer | - | Sí |
| `fecha_apertura` | DateTime | - | Sí |
| `atendido2` | Text (1) | 1 | Sí |
| `atendido3` | Text (1) | 1 | Sí |
| `atendido4` | Text (1) | 1 | Sí |
| `fecha_atendido` | DateTime | - | Sí |
| `fecha_atendido2` | DateTime | - | Sí |
| `fecha_atendido3` | DateTime | - | Sí |
| `fecha_atendido4` | DateTime | - | Sí |
| `factura_electronica_codigo_mensaje_recibido` | Text (255) | 255 | Sí |
| `factura_electronica_mensaje_recibido` | Text (255) | 255 | Sí |
| `referencia_1` | Text (50) | 50 | Sí |
| `referencia_2` | Text (50) | 50 | Sí |
| `referencia_3` | Text (50) | 50 | Sí |

**Posibles claves naturales:** `codigo_cliente`, `factura_electronica_codigo_mensaje_recibido`, `numero`, `numero_solicitud`

## detalle_cuentas_por_cobrar_gastos

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `item` | Long Integer | - | Sí |
| `num_factura` | Long Integer | - | Sí |
| `cod_producto` | Text (20) | 20 | Sí |
| `id_tercero` | Text (20) | 20 | Sí |
| `entrega` | Text (255) | 255 | Sí |
| `cantidad` | Double | - | Sí |
| `valor_unitario` | Double | - | Sí |
| `impuesto` | Double | - | Sí |
| `descuento` | Double | - | Sí |
| `costo_promedio` | Double | - | Sí |
| `id_vendedor` | Text (20) | 20 | Sí |
| `comision` | Double | - | Sí |
| `peso_recipiente1` | Double | - | Sí |
| `numero_recipientes1` | Double | - | Sí |
| `peso_recipiente2` | Double | - | Sí |
| `numero_recipientes2` | Double | - | Sí |
| `peso_recipiente3` | Double | - | Sí |
| `numero_recipientes3` | Double | - | Sí |
| `impuesto2` | Double | - | Sí |
| `impuesto3` | Double | - | Sí |
| `codigo_impuesto1` | Double | - | Sí |
| `codigo_impuesto2` | Double | - | Sí |
| `codigo_impuesto3` | Double | - | Sí |
| `base_no_gravada` | Double | - | Sí |
| `seriales` | Memo/Hyperlink (255) | 255 | Sí |
| `id_promocion_aplicada` | Long Integer | - | Sí |
| `id_bodega_inventario` | Text (20) | 20 | Sí |

**Posibles claves naturales:** `codigo_impuesto1`, `codigo_impuesto2`, `codigo_impuesto3`, `numero_recipientes1`, `numero_recipientes2`, `numero_recipientes3`

## bodegas

> Error al leer la tabla: Command '['mdb-schema', '-T', 'bodegas', '/workspace/pos_system/bd1.mdb']' returned non-zero exit status 1.

## detalle_producto_bodega

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `codigo_producto` | Text (255) | 255 | Sí |
| `id_bodega` | Long Integer | - | Sí |

**Posibles claves naturales:** `codigo_producto`

## usuarios

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `id_usuario` | Text (255) | 255 | No |
| `contrasena` | Text (255) | 255 | Sí |
| `codigo_de_seguridad` | Text (255) | 255 | Sí |
| `fecha_expiracion_codigo_seguridad` | DateTime | - | Sí |
| `observaciones` | Text (255) | 255 | Sí |
| `id_tercero_relacionado` | Text (255) | 255 | Sí |

**Posibles claves naturales:** `codigo_de_seguridad`, `fecha_expiracion_codigo_seguridad`

## vendedores

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `consecutivo` | Long Integer | - | Sí |
| `codigo` | Text (20) | 20 | No |
| `nit` | Text (15) | 15 | Sí |
| `nombre` | Text (50) | 50 | Sí |
| `direccion` | Text (50) | 50 | Sí |
| `ciudad` | Text (50) | 50 | Sí |
| `telefono` | Text (50) | 50 | Sí |
| `celular` | Text (50) | 50 | Sí |
| `e_mail` | Text (100) | 100 | Sí |
| `asesor` | Text (50) | 50 | Sí |
| `observaciones` | Text (255) | 255 | Sí |
| `contacto_directo` | Text (50) | 50 | Sí |
| `Retirado` | Text (1) | 1 | Sí |
| `retirado_por_usuario` | Text (50) | 50 | Sí |
| `motivo_retiro` | Text (255) | 255 | Sí |
| `porcentaje_descuento` | Double | - | Sí |
| `cupo_maximo_credito` | Double | - | Sí |
| `reteica` | Double | - | Sí |
| `retefuente` | Double | - | Sí |
| `propiedad_retencion` | Text (255) | 255 | Sí |
| `saldo_puntos_acumulados` | Double | - | Sí |
| `fecha_nacimiento` | DateTime | - | Sí |
| `lista_precios_productos` | Text (50) | 50 | Sí |
| `tipo_documento` | Text (50) | 50 | Sí |
| `tipo_contribuyente` | Text (50) | 50 | Sí |
| `regimen_contable` | Text (255) | 255 | Sí |
| `departamento` | Text (255) | 255 | Sí |
| `pais` | Text (50) | 50 | Sí |
| `razon_social` | Text (255) | 255 | Sí |
| `codigo_municipio` | Text (20) | 20 | Sí |
| `actividad_economica` | Text (50) | 50 | Sí |
| `barrio` | Text (255) | 255 | Sí |
| `zona` | Text (50) | 50 | Sí |

**Posibles claves naturales:** `codigo`, `codigo_municipio`, `nit`, `tipo_documento`

## detalle_usuario_permiso

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `id_detalle` | Long Integer | - | Sí |
| `usuario` | Text (255) | 255 | Sí |
| `indice_permiso_permitido` | Long Integer | - | Sí |

## movimientos

| Columna | Tipo OLEDB | Longitud | Permite nulos |
| --- | --- | --- | --- |
| `numero` | Long Integer | - | Sí |
| `codigo_producto` | Text (50) | 50 | Sí |
| `cantidad` | Double | - | Sí |
| `costo` | Double | - | Sí |
| `tipo_movimiento` | Text (50) | 50 | Sí |
| `accion_generadora` | Text (50) | 50 | Sí |
| `tipo_documento` | Text (50) | 50 | Sí |
| `numero_documento` | Long Integer | - | Sí |
| `fecha` | DateTime | - | Sí |
| `usuario` | Text (20) | 20 | Sí |
| `observaciones` | Text (100) | 100 | Sí |
| `nit_tercero` | Text (20) | 20 | Sí |
| `saldo_inicial` | Double | - | Sí |
| `costo_final` | Double | - | Sí |
| `id_bodega` | Text (20) | 20 | Sí |
| `saldo_inicial_bodega` | Double | - | Sí |

**Posibles claves naturales:** `codigo_producto`, `numero`, `numero_documento`, `tipo_documento`