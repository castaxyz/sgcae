# Contexto del caso de estudio
    Una empresa dedicada a la prestación de servicios logísticos y de almacenamiento,
    cuenta con una sede principal y varias áreas de acceso restringido. Actualmente, el
    ingreso y la salida de los empleados se registran mediante planillas físicas y archivos de
    hojas de cálculo administrados por el personal de seguridad.
    Este procedimiento presenta problemas como registros duplicados, dificultad para
    verificar la identidad de los empleados, falta de trazabilidad de los ingresos y salidas,
    demora en la generación de informes y ausencia de mecanismos para impedir el ingreso
    de empleados cuyo vínculo laboral se encuentre inactivo.
    Ante esta situación, la gerencia solicita desarrollar un Sistema de Gestión y Control de
    Acceso de Empleados (SGCAE) que permita administrar la información de los empleados,
    controlar sus credenciales, registrar los movimientos de entrada y salida, y consultar el
    historial de accesos de manera centralizada.
    La solución deberá construirse como una aplicación backend con una API REST que
    permita a otros sistemas consumir sus funcionalidades. La información deberá
    almacenarse en SQL Server y la implementación deberá evidenciar el uso de principios
    de arquitectura limpia, modelado de dominio (DDD)y separación entre operaciones de
    consulta y modificación (CQRS).

# Objetivos

## Objetivo general
    Desarrollar un backend empresarial para gestionar y controlar los accesos de los
    empleados a las instalaciones de una organización, aplicando Clean Architecture, DDD y
    CQRS, con persistencia mediante Entity Framework Core sobre SQL Server, exposición de
    servicios mediante ASP.NET Core Web API y gestión colaborativa del código fuente con
    GitHub.

## Objetivos específicos
    - Diseñar una solución organizada según las capas y dependencias de Clean
    Architecture.
    - Identificar el dominio del negocio, sus entidades, objetos de valor, reglas e
    invariantes.
    -​ Implementar los casos de uso mediante CQRS, separando comandos y consultas.
    -​ Persistir y consultar información usando Entity Framework Core y SQL Server.
    -​ Publicar endpoints REST documentados y verificables.
    -​ Gestionar el desarrollo mediante repositorio git en GitHub
    -​ Presentar evidencias verificables del funcionamiento de los casos de uso y de la
    API.

# Requerimientos funcionales

    - RF-01. Gestión de empleados
    El sistema permitirá registrar, consultar, actualizar y desactivar empleados. Cada
    empleado deberá tener identificación única, nombres, apellidos, correo electrónico,
    cargo, área organizacional y estado laboral.
    Resultado esperado: empleados identificables y con información actualizada.

    - RF-02. Gestión de credenciales
    El sistema permitirá asignar credenciales de acceso a los empleados, consultar su
    estado, bloquearlas y revocarlas. Cada credencial deberá tener un código único y un
    estado que permita determinar si puede utilizarse.
    Resultado esperado: control de credenciales vigentes, bloqueadas o revocadas.

    - RF-03. Registro de entradas y salidas
    El sistema deberá recibir una solicitud de acceso con el código de la credencial, el punto
    de acceso y el tipo de movimiento (entrada o salida). Deberá validar las condiciones del
    empleado y registrar el resultado de la operación, incluyendo los accesos rechazados.
    Resultado esperado: historial trazable de movimientos autorizados y rechazados.

    - RF-04. Consulta del historial de accesos
    El sistema permitirá consultar los movimientos de acceso por empleado, rango de fechas,
    punto de acceso y resultado de validación. Las consultas deberán permitir paginación
    para evitar recuperar grandes volúmenes de información de una sola vez.

    - RF-06. Administración de puntos de acceso
    El sistema permitirá registrar y consultar los puntos de acceso físicos de la organización,
    como recepción, parqueadero, bodega y áreas restringidas, identificando cuáles se
    encuentran habilitados.

# Entregable
Debe de compartir el link de repositorio público en GitHub con el desarrollo del caso de estudio.