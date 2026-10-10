# TpBlockchainPW3 — Notarización de documentos con Blockchain

Trabajo Práctico de Investigación · Programación Web 3 · UNLaM · Grupo 7

> **Tema:** Blockchain más allá de las criptomonedas: contratos inteligentes y seguridad en transacciones con tecnologías .NET

Aplicación web ASP.NET Core MVC que permite **registrar documentos en una blockchain** (se guarda solo su huella digital SHA-256) y **verificar públicamente** si un archivo es idéntico al registrado originalmente.

---

## Índice

1. [Requisitos previos](#1-requisitos-previos)
2. [Clonar el repositorio](#2-clonar-el-repositorio)
3. [Estructura del repositorio](#3-estructura-del-repositorio)
4. [Instalar la herramienta de Entity Framework](#4-instalar-la-herramienta-de-entity-framework)
5. [Base de datos](#5-base-de-datos)
6. [Ejecutar la aplicación](#6-ejecutar-la-aplicación)
7. [Verificar que todo funciona](#7-verificar-que-todo-funciona)
8. [Trabajar con migraciones](#8-trabajar-con-migraciones)
9. [Problemas comunes](#9-problemas-comunes)
10. [Flujo de trabajo con Git](#10-flujo-de-trabajo-con-git)
11. [Tecnologías](#11-tecnologías)

---

## 1. Requisitos previos

Instalá lo siguiente **antes** de clonar el proyecto:

| Herramienta | Versión | Para qué |
|---|---|---|
| [Visual Studio 2026](https://visualstudio.microsoft.com/) (Community alcanza) | Con soporte para .NET 10 | Editor e IDE |
| Carga de trabajo **"Desarrollo de ASP.NET y web"** | — | Se elige en el Visual Studio Installer |
| [SDK de .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) | 10.0.x | Compilar y ejecutar el proyecto |
| **SQL Server Express LocalDB** | — | Base de datos local |
| [Git](https://git-scm.com/) (y opcionalmente [Fork](https://git-fork.com/)) | — | Control de versiones |
| [SQL Server Management Studio](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) | Opcional | Ver la base de datos fuera de Visual Studio |

### Cómo verificar que está todo

Abrí una terminal (PowerShell) y ejecutá:

```powershell
dotnet --list-sdks
```

Tiene que aparecer al menos una línea que empiece con `10.0`.

```powershell
sqllocaldb info
```

Tiene que aparecer `MSSQLLocalDB`. Si el comando no existe, abrí el **Visual Studio Installer → Modificar → Componentes individuales**, buscá **SQL Server Express LocalDB**, tildalo e instalalo.

> ⚠️ **El proyecto usa .NET 10.** Si tenés solo .NET 8 instalado, el proyecto **no va a compilar**.

---

## 2. Clonar el repositorio

**Opción A — desde Visual Studio:**

1. Abrí Visual Studio → **Clonar un repositorio**.
2. Pegá la URL del repo: `https://github.com/<usuario>/tp-blockchain-pw3.git`
3. Elegí una carpeta **sin tildes, eñes ni espacios raros** (por ejemplo `C:\Users\<tu-usuario>\source\repos\`).
4. Clic en **Clonar**.

**Opción B — desde la terminal:**

```powershell
cd C:\Users\<tu-usuario>\source\repos
git clone https://github.com/<usuario>/tp-blockchain-pw3.git
```

Después abrí la solución `TpBlockchainPW3\TpBlockchainPW3.slnx` con Visual Studio.

---

## 3. Estructura del repositorio

```text
tp-blockchain-pw3/                      ← raíz del repositorio
├── README.md                           ← este archivo
├── .gitignore
├── contracts/                          ← contrato inteligente (Solidity) y su ABI
├── database/
│   └── script.sql                      ← script SQL generado desde las migraciones
└── TpBlockchainPW3/
    ├── TpBlockchainPW3.slnx            ← la solución (abrir este archivo)
    └── TpBlockchainPW3.Web/            ← el proyecto ASP.NET Core MVC
        ├── Areas/                      ← páginas de login y registro (Identity)
        ├── Controllers/                ← controladores MVC
        ├── Data/                       ← DbContext y migraciones de EF Core
        ├── Models/                     ← entidades (Documento, enums)
        ├── Options/                    ← clases de configuración (BlockchainOptions)
        ├── Services/                   ← lógica de negocio e integraciones
        │   └── Interfaces/             ← contratos de los servicios
        ├── Storage/                    ← archivos guardados por la app (NO se sube a Git)
        ├── ViewModels/                 ← modelos para vistas y formularios
        ├── Views/                      ← vistas Razor
        ├── wwwroot/                    ← CSS, JS, Bootstrap
        ├── appsettings.json
        └── Program.cs
```

> 💡 **Solución vs. proyecto:** la solución (`.slnx`) es solo un contenedor; el proyecto (`.csproj`) es lo que se compila. Hay **un solo proyecto**: `TpBlockchainPW3.Web`.

---

## 4. Instalar la herramienta de Entity Framework

Se hace **una sola vez por computadora**. En una terminal:

```powershell
dotnet tool install --global dotnet-ef --version 10.*
```

Si te dice que ya está instalada, actualizala:

```powershell
dotnet tool update --global dotnet-ef --version 10.*
```

Verificá:

```powershell
dotnet ef --version
```

Tiene que mostrar una versión que empiece con `10.0`.

> La versión de `dotnet-ef` **tiene que coincidir** con la versión principal de Entity Framework Core del proyecto (10.x).

---

## 5. Base de datos

La base de datos es **SQL Server LocalDB** y se crea **con las migraciones de Entity Framework Core**, no con un script escrito a mano. El esquema lo define el código C# (las entidades en `Models` y el `ApplicationDbContext` en `Data`).

### 5.1 Abrir la terminal en la carpeta del proyecto

En Visual Studio, en el **Explorador de soluciones**: clic derecho sobre **TpBlockchainPW3.Web** → **Abrir en terminal**.

El prompt tiene que terminar en la carpeta del proyecto:

```text
PS C:\...\tp-blockchain-pw3\TpBlockchainPW3\TpBlockchainPW3.Web>
```

> Si abriste la terminal con **Ver → Terminal**, primero entrá a la carpeta del proyecto con `cd TpBlockchainPW3.Web`.

### 5.2 Crear la base

```powershell
dotnet ef database update
```

Tiene que terminar con la palabra **`Done.`**

Esto crea la base `TpBlockchainPW3` en LocalDB con todas las tablas (usuarios de Identity, documentos, etc.).

> 💡 **Usá la terminal, no la "Consola del Administrador de paquetes".** La consola de NuGet a veces oculta el error real y solo muestra un `RemoteException` sin explicación.

### 5.3 Cadena de conexión

Está en `TpBlockchainPW3.Web/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TpBlockchainPW3;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

- `Server=(localdb)\\mssqllocaldb` → la instancia de LocalDB (la doble barra `\\` es obligatoria en JSON).
- `Database=TpBlockchainPW3` → nombre de la base.
- `Trusted_Connection=True` → se entra con el usuario de Windows, sin usuario ni contraseña.

> ⚠️ **No modifiques esta cadena en el repo.** Todos usamos LocalDB para que funcione igual en las 7 máquinas.

### 5.4 Ver la base de datos

**Desde Visual Studio:** **Ver → Explorador de objetos de SQL Server** → **SQL Server → (localdb)\MSSQLLocalDB → Bases de datos → TpBlockchainPW3 → Tablas**.

**Desde SQL Server Management Studio (SSMS):**

| Campo | Valor |
|---|---|
| Tipo de servidor | Motor de base de datos |
| Nombre del servidor | `(localdb)\MSSQLLocalDB` |
| Autenticación | Autenticación de Windows |
| Cifrado | Opcional, o tildá "Confiar en el certificado del servidor" |

### 5.5 (Opcional) Usar otra instancia de SQL Server

Si preferís usar **SQL Server Express o Developer** en tu máquina, **no cambies `appsettings.json`**. Configurá tu cadena en **User Secrets** (solo afecta a tu computadora):

```powershell
# SQL Server Express (instancia SQLEXPRESS)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=TpBlockchainPW3;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"

# SQL Server Developer (instancia por defecto)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=TpBlockchainPW3;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Después corré `dotnet ef database update`.

> `TrustServerCertificate=True` es necesario porque el driver de SQL Server cifra la conexión por defecto y una instalación local usa un certificado no oficial.

### 5.6 Script SQL

En `database/script.sql` está el script SQL equivalente a todas las migraciones. **No se edita a mano**: se regenera con EF Core cada vez que se agrega una migración (ver [sección 8](#8-trabajar-con-migraciones)).

Para crear la base manualmente con el script (por ejemplo en SSMS):

```sql
CREATE DATABASE TpBlockchainPW3;
GO
USE TpBlockchainPW3;
GO
-- Después, abrir y ejecutar database/script.sql
```

> En el uso normal **no hace falta**: `dotnet ef database update` hace todo.

---

## 6. Ejecutar la aplicación

En Visual Studio, presioná **F5** (o el botón ▶ verde con el perfil **https**).

La primera vez, Visual Studio puede preguntarte si confiás en el **certificado de desarrollo HTTPS**. Respondé **Sí** a todo.

Se abre el navegador en una dirección del estilo `https://localhost:7035`.

> Al detener la ejecución, en la ventana **Salida** aparece `terminó con código 4294967295 (0xffffffff)`. **Es normal**, no es un error.

---

## 7. Verificar que todo funciona

1. Tenés que ver la página de inicio con los links **Register** y **Login** arriba a la derecha.
2. Clic en **Register** y creá un usuario de prueba.
3. Si aparece una pantalla para confirmar el email, hacé clic en el link de confirmación que se muestra ahí mismo (en desarrollo no se envían correos reales).
4. Iniciá sesión con **Login**.
5. Arriba a la derecha tiene que aparecer **Hello tu@correo.com!** y **Logout**.

✅ Si llegaste hasta acá, tu entorno está listo.

---

## 8. Trabajar con migraciones

Una **migración** es un archivo C# (en `Data/Migrations`) que describe un cambio en la base: crear una tabla, agregar una columna, un índice, etc.

### Cuando hacés `git pull` y hay migraciones nuevas

```powershell
dotnet ef database update
```

### Cuando modificás un modelo (por ejemplo agregás una propiedad a `Documento`)

Desde la terminal del proyecto:

```powershell
# 1. Crear la migración (usar un nombre descriptivo, en PascalCase)
dotnet ef migrations add AgregarCampoX

# 2. Aplicarla a tu base local
dotnet ef database update

# 3. Regenerar el script SQL del repo
dotnet ef migrations script --idempotent -o ..\..\database\script.sql
```

Commiteá **juntos** el cambio en el modelo, la migración generada y el script actualizado.

> ⚠️ **Reglas para no romperle la base a nadie:**
> - Nunca edites ni borres una migración que ya está en `main`.
> - Si dos personas crean migraciones al mismo tiempo, coordinen quién mergea primero; el otro hace `git pull` y vuelve a generar la suya.
> - Si te equivocaste en una migración que **todavía no subiste**, podés deshacerla con `dotnet ef migrations remove`.

---

## 9. Problemas comunes

### ❌ `Una directiva de Control de aplicaciones bloqueó este archivo. (0x800711C7)`

**Causa:** el **Control inteligente de aplicaciones** (*Smart App Control*) de Windows 11 bloquea la DLL que genera la compilación, porque no tiene firma digital.

**Solución:**

1. Abrí **Seguridad de Windows**.
2. **Control de aplicaciones y navegador** → **Configuración del Control inteligente de aplicaciones**.
3. Cambialo a **Desactivado**.
4. Volvé a correr `dotnet ef database update`.

> ⚠️ Leé el aviso de Windows antes de desactivarlo: en algunas versiones de Windows 11 no se puede volver a activar sin reinstalar el sistema. El antivirus (Microsoft Defender) sigue funcionando igual.
>
> Si la computadora es del trabajo o de una institución y no podés cambiar esta opción, pedile al área de sistemas una excepción para la carpeta `source\repos`. Si usás un antivirus de terceros, agregá una exclusión para esa carpeta.

### ❌ `Update-Database` falla con `ErrorActionPreference ... RemoteException`

**Causa:** la Consola del Administrador de paquetes oculta el error real.

**Solución:** usá la terminal con `dotnet ef database update --verbose` (ver [sección 5](#5-base-de-datos)). Al final de la salida aparece el mensaje verdadero.

### ❌ `sqllocaldb` no se reconoce como comando

**Causa:** LocalDB no está instalado.

**Solución:** Visual Studio Installer → Modificar → Componentes individuales → **SQL Server Express LocalDB**.

### ❌ No se puede conectar a `(localdb)\mssqllocaldb`

**Solución:** arrancá la instancia manualmente:

```powershell
sqllocaldb start MSSQLLocalDB
```

### ❌ Error de certificado al conectar a SQL Server Express/Developer

**Causa:** falta `TrustServerCertificate=True` en la cadena de conexión.

**Solución:** ver [sección 5.5](#55-opcional-usar-otra-instancia-de-sql-server).

### ❌ El proyecto no compila: `The current .NET SDK does not support targeting .NET 10.0`

**Causa:** no tenés instalado el SDK de .NET 10 o tu Visual Studio es una versión anterior.

**Solución:** instalá el [SDK de .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) y actualizá Visual Studio.

### ❌ Errores raros de versiones de paquetes

**Causa:** alguien actualizó un paquete de Entity Framework a otra versión principal.

**Solución:** todos los paquetes `Microsoft.EntityFrameworkCore.*` y `Microsoft.AspNetCore.Identity.EntityFrameworkCore` tienen que estar en **10.x**. No uses "Actualizar todo" en NuGet sin avisar al grupo.

### ❌ Git: `Permission denied` sobre archivos de la carpeta `.vs`

**Causa:** Git intenta subir la carpeta `.vs` (caché local de Visual Studio, bloqueada mientras está abierto). Pasa si falta el `.gitignore` en la raíz del repo.

**Solución:** verificá que exista `.gitignore` en la raíz. Si no está, desde la raíz del repo:

```powershell
dotnet new gitignore
git reset
```

Y volvé a stagear. **Nunca subas** `.vs/`, `bin/` ni `obj/`.

### ❌ La ruta del proyecto tiene tildes o caracteres especiales

Algunas herramientas fallan con rutas como `C:\Users\...\Programación\...`. Mové el repo a una ruta simple, como `C:\Repos\tp-blockchain-pw3`.

---

## 10. Flujo de trabajo con Git

- **Nunca trabajar directo sobre `main`.** Creá una rama por funcionalidad:

  ```powershell
  git checkout main
  git pull
  git checkout -b feature/nombre-de-la-funcionalidad
  ```

  Ejemplos: `feature/modelo-documento`, `feature/blockchain-service`, `feature/verificacion`.

- Al terminar, subí la rama y abrí un **Pull Request** hacia `main` en GitHub para que otro integrante lo revise.
- Después de que se mergea un PR, todos hacen `git pull` en `main` y, si hay migraciones nuevas, `dotnet ef database update`.
- Git **no registra carpetas vacías**: una carpeta nueva aparece en el repo recién cuando tiene al menos un archivo.

### Qué NO se sube al repositorio

| Qué | Por qué |
|---|---|
| `.vs/`, `bin/`, `obj/` | Archivos locales y de compilación; los regenera Visual Studio |
| `Storage/` | Archivos subidos y PDFs generados por la app |
| Claves privadas, API keys, cadenas de conexión personales | Seguridad: van en User Secrets |

### 🔐 Secretos: nunca subir claves al repositorio

La clave privada de la cuenta de blockchain, la URL del proveedor (Infura/Alchemy) y la dirección del contrato **no van en `appsettings.json`**. Se guardan con **User Secrets**, que quedan fuera de la carpeta del repo:

- Visual Studio: clic derecho en el proyecto → **Administrar secretos de usuario**.
- Terminal (en la carpeta del proyecto):

  ```powershell
  dotnet user-secrets set "Blockchain:RpcUrl" "https://sepolia.infura.io/v3/<API_KEY>"
  dotnet user-secrets set "Blockchain:PrivateKey" "0x..."
  dotnet user-secrets set "Blockchain:ContractAddress" "0x..."
  ```

Los valores se comparten por un canal privado del grupo, **nunca** por GitHub.

---

## 11. Tecnologías

| Tecnología | Uso |
|---|---|
| ASP.NET Core MVC (.NET 10) | Aplicación web |
| ASP.NET Core Identity | Registro e inicio de sesión |
| Entity Framework Core 10 + SQL Server LocalDB | Base de datos (enfoque *code-first* con migraciones) |
| Bootstrap | Interfaz de usuario |
| Nethereum | Comunicación con la blockchain (red de prueba Sepolia) |
| Solidity | Contrato inteligente de notarización |
| QuestPDF / QRCoder | Generación de certificados PDF y códigos QR |

---

## Integrantes — Grupo 7

- Mora Cicero
- Santiago Gismondi
- Danilo Ticak
- Pascale Ignacio
- Lucas Marron
- Anna Sol Manfredi
- Joaquín Ramiro Gielis
