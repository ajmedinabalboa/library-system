# 📊 Datos de Prueba (Seed Data)

Este documento describe los datos de prueba incluidos en la base de datos.

## 👥 Usuarios

| Email | Password | Role | Username |
|-------|----------|------|----------|
| admin@library.com | `Admin123!` | Admin | admin |
| user@library.com | `User123!` | User | testuser |

**Nota:** Las contraseñas están hasheadas usando `PasswordHasher<User>` de ASP.NET Core Identity.

## 📚 Libros

| Título | Fecha de Publicación | Autor(es) |
|--------|---------------------|-----------|
| Clean Code: A Handbook of Agile Software Craftsmanship | 01/08/2008 | Robert C. Martin |
| Clean Architecture: A Craftsman's Guide to Software Structure | 20/09/2017 | Robert C. Martin |
| Refactoring: Improving the Design of Existing Code | 08/07/1999 | Martin Fowler, Kent Beck |
| Domain-Driven Design: Tackling Complexity in the Heart of Software | 30/08/2003 | Eric Evans |
| Test Driven Development: By Example | 18/11/2002 | Kent Beck |
| The Pragmatic Programmer: Your Journey to Mastery | 30/10/1999 | Andrew Hunt |

## ✍️ Autores

1. **Robert C. Martin** (Uncle Bob)
   - Clean Code
   - Clean Architecture

2. **Martin Fowler**
   - Refactoring (co-autor)

3. **Eric Evans**
   - Domain-Driven Design

4. **Kent Beck**
   - Refactoring (co-autor)
   - Test Driven Development

5. **Andrew Hunt**
   - The Pragmatic Programmer

## 🔧 Cómo Aplicar los Datos de Prueba

Los datos de prueba se aplican automáticamente cuando ejecutas las migraciones:

### 1. Crear la Migración

```bash
cd backend/src/LibrarySystem.Infrastructure

dotnet ef migrations add InitialCreateWithSeedData \
  --startup-project ../LibrarySystem.WebAPI \
  --context ApplicationDbContext \
  --output-dir Persistence/Migrations
```

### 2. Aplicar la Migración

```bash
dotnet ef database update \
  --startup-project ../LibrarySystem.WebAPI \
  --context ApplicationDbContext
```

### 3. Verificar los Datos

Puedes verificar que los datos se insertaron correctamente:

```sql
-- Contar usuarios
SELECT COUNT(*) FROM "Users";
-- Resultado esperado: 2

-- Contar libros
SELECT COUNT(*) FROM "Books";
-- Resultado esperado: 6

-- Contar autores
SELECT COUNT(*) FROM "Authors";
-- Resultado esperado: 5

-- Ver libros con sus autores
SELECT b."Title", a."Name"
FROM "Books" b
JOIN "BookAuthors" ba ON b."Id" = ba."BookId"
JOIN "Authors" a ON ba."AuthorId" = a."Id"
ORDER BY b."Title";
```

## 🧪 Pruebas con los Datos

### Login con Usuario Admin

**Request:**
```bash
curl -k -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@library.com",
    "password": "Admin123!"
  }'
```

**Response:**
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "abc123...",
  "expiresIn": 900
}
```

### Login con Usuario Normal

**Request:**
```bash
curl -k -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@library.com",
    "password": "User123!"
  }'
```

### Listar Todos los Libros

```bash
curl -k -X GET https://localhost:5001/api/books \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### Buscar Libros por Título

```bash
# Buscar libros que contengan "Clean"
curl -k -X GET "https://localhost:5001/api/books?title=Clean" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### Filtrar por Múltiples Autores

```bash
# Buscar libros con al menos 2 autores
curl -k -X GET "https://localhost:5001/api/books?minAuthors=2" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### Ordenar por Fecha de Publicación

```bash
# Ordenar por fecha descendente (más recientes primero)
curl -k -X GET "https://localhost:5001/api/books?sortBy=publicationDate&sortOrder=desc" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

## 🔄 Restablecer los Datos de Prueba

Si necesitas restablecer la base de datos con los datos de prueba originales:

```bash
# Eliminar la base de datos
dotnet ef database drop --force \
  --startup-project ../LibrarySystem.WebAPI \
  --context ApplicationDbContext

# Volver a crear con datos de prueba
dotnet ef database update \
  --startup-project ../LibrarySystem.WebAPI \
  --context ApplicationDbContext
```

## 📝 Notas Importantes

1. **IDs Fijos**: Los datos de prueba usan GUIDs predefinidos para facilitar las pruebas y referencias.

2. **Contraseñas**: Las contraseñas están hasheadas usando el mismo algoritmo que usará la aplicación en producción.

3. **Fechas**: Todas las fechas de creación están configuradas en UTC para consistencia.

4. **Relaciones**: Las relaciones many-to-many entre libros y autores están completamente configuradas.

5. **Producción**: En un entorno de producción, considera:
   - No incluir usuarios de prueba
   - Usar variables de entorno para contraseñas
   - Agregar un flag para habilitar/deshabilitar seed data

## 🎯 Frontend - Credenciales de Prueba

Usa estas credenciales en el frontend (http://localhost:3000/login):

- **Admin:** admin@library.com / Admin123!
- **Usuario:** user@library.com / User123!

Después de iniciar sesión, podrás:
- ✅ Ver los 6 libros de prueba
- ✅ Buscar y filtrar por título
- ✅ Ver autores asociados
- ✅ Crear nuevos libros
- ✅ Editar libros existentes
- ✅ Eliminar libros
