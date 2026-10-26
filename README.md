# SGCAE — Sistema de Gestión y Control de Acceso de Empleados

API REST desarrollada con .NET 10, Clean Architecture, DDD, CQRS, MediatR y EF Core sobre SQL Server.

---

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)

---

## 1. Base de datos (SQL Server en Docker)

```bash
docker run \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Sgcae_2026!" \
  -p 1433:1433 \
  --name sgcae-sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Esperar ~15 segundos y verificar que está corriendo:

```bash
docker ps | grep sgcae-sqlserver
```

---

## 2. Crear la migración inicial (solo la primera vez)

```bash
cd /home/casta/Documents/2026-2/distribuida/sgcae

dotnet ef migrations add InitialCreate \
  --project src/SGCAE.Infrastructure \
  --startup-project src/SGCAE.WebApi
```

> Solo se ejecuta una vez. Si ya existe la carpeta `src/SGCAE.Infrastructure/Migrations`, omitir este paso.

---

## 3. Correr la API

```bash
cd src/SGCAE.WebApi
dotnet run
```

La API aplica las migraciones e inserta datos de prueba automáticamente al iniciar.

| URL | Descripción |
|-----|-------------|
| http://localhost:5158 | Swagger UI |
| http://localhost:5158/api/employees | Empleados |
| http://localhost:5158/api/credentials | Credenciales |
| http://localhost:5158/api/access-points | Puntos de acceso |
| http://localhost:5158/api/access/movements | Registrar acceso |
| http://localhost:5158/api/access/history | Historial paginado |

---

## 4. Datos de prueba (seed automático)

Al iniciar, el sistema crea:

| Elemento | Código / Doc | Estado |
|----------|-------------|--------|
| Empleado Juan Pérez | `10101001` | Activo |
| Empleado María López | `20202002` | **Inactivo** |
| Credencial activa | `CARD-ACT-001` | Active → Juan |
| Credencial bloqueada | `CARD-BLK-002` | **Blocked** → Juan |
| Credencial revocada | `CARD-REV-003` | **Revoked** |
| Credencial de inactivo | `CARD-INA-004` | Active → María |
| Punto Recepción | `REC-01` | Habilitado |
| Punto Bodega | `BOD-01` | Habilitado |
| Punto Parqueadero | `PRK-01` | Habilitado |
| Punto Servidores | `RES-01` | Habilitado |

---

## 5. Pruebas HTTP (REST Client)

Abrir `SGCAE.http` en la raíz del repositorio con VS Code (extensión REST Client) o Visual Studio 2022.

Actualizar las variables `@employeeJuanId`, `@accessPointRecepcionId`, etc. con los IDs reales devueltos por los POST antes de ejecutar las siguientes peticiones.

---

## 6. Pruebas unitarias

```bash
dotnet test
```

Resultado esperado: **52 tests — 100% passed**.

---

## 7. Limpieza y destrucción

### Detener la API
`Ctrl + C` en la terminal donde corre `dotnet run`.

### Detener el contenedor de SQL Server
```bash
docker stop sgcae-sqlserver
```

### Eliminar el contenedor (conserva la imagen)
```bash
docker rm sgcae-sqlserver
```

### Eliminar también la imagen de SQL Server
```bash
docker rmi mcr.microsoft.com/mssql/server:2022-latest
```

### Limpiar binarios del proyecto
```bash
dotnet clean
```

### Destrucción completa (contenedor + imagen + binarios)
```bash
docker stop sgcae-sqlserver && docker rm sgcae-sqlserver
docker rmi mcr.microsoft.com/mssql/server:2022-latest
dotnet clean
```
