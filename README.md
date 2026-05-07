# BIK - Auth Service

Microservicio de autenticación para el ecosistema bancario BIK (Banco Informático Kinal). Desarrollado en C# con .NET 8, este servicio maneja el registro seguro de credenciales, hashing de contraseñas usando BCrypt y la emisión de JSON Web Tokens (JWT) para asegurar las transacciones a través de la plataforma.

## Tecnologías
- C# / .NET 8
- MongoDB Driver para persistencia de datos de seguridad
- BCrypt para hashing
- JWT (JSON Web Tokens)

## Configuración y Ejecución
Este servicio está diseñado para funcionar en un entorno Dockerizado.
Ejecuta `docker-compose up -d auth-service` desde la carpeta principal del proyecto.

## Endpoints Principales
- `POST /api/auth/register-credentials`: Almacena de forma segura las contraseñas de los nuevos usuarios (se comunica internamente con Server-Admin).
- `POST /api/auth/login`: Valida las credenciales y devuelve un token JWT con tiempo de expiración (1 hora).
