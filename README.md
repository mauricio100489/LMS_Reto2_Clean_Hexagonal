# LMS_Reto2_Clean_Hexagonal
Reto1: Curso de Modernización de Aplicaciones - Lite Thinking - Aplicando Clean Hexagonal

Instrucciones: Crearemos una API RESTful que ejemplifique los principios de Arquitectura Hexagonal, organizando el código en dominio, puertos y adaptadores.

Tarea:
1) Crea la solución y capas (20 puntos)
• Capa de Dominio
• Capa de Aplicación
• Capa de Adapters
• Crea las referencias entre los proyectos que respeten Hexagonal

2) Implementa la capa de Dominio (20 puntos)
• Models
• Ports

3) Implementa capa de Aplicación (20 puntos)
• Services

4) Implementa capa de Adapters (20 puntos)
• Inbound
• Outbound

5) Pruebas de métodos (GET y POST) (20 puntos)
• Pantallas de ejecución y respuesta


#Descripción
API RESTful que ejemplifica los principios de Arquitectura Hexagonal (también conocida como Ports & Adapters), con autenticación JWT completa, organizando el código en capas independientes y desacopladas.

#Características
✅ Arquitectura Hexagonal completa (Domain, Application, Adapters)
🔐 Autenticación JWT con login y registro
👥 Sistema de roles (Admin y User)
🛡️ Endpoints protegidos con autorización
📦 CRUD completo de productos
🧪 Swagger UI con soporte JWT
🏛️ Separación clara de responsabilidades
🔌 Puertos e interfaces bien definidos


#Capas de la Arquitectura
┌─────────────────────────────────────────────────────────┐
│                    Adapters (Inbound)                   │
│              API REST Controllers + DTOs                │
│           AuthController | ProductsController           │
└─────────────────────┬───────────────────────────────────┘
                      │ usa
                      ▼
┌─────────────────────────────────────────────────────────┐
│                   Application Layer                     │
│                  Services (Use Cases)                   │
│             AuthService | ProductService                │
│    Implementa: IAuthService | IProductService           │
└─────────────────────┬───────────────────────────────────┘
                      │ usa
                      ▼
┌─────────────────────────────────────────────────────────┐
│                     Domain Layer                        │
│              Models + Ports (Interfaces)                │
│         User | Product | IUserRepository |              │
│    IProductRepository | IJwtTokenGenerator              │
└─────────────────────┬───────────────────────────────────┘
                      │ implementado por
                      ▼
┌─────────────────────────────────────────────────────────┐
│                   Adapters (Outbound)                   │
│          Persistence + Security Implementations         │
│    UserRepository | ProductRepository | JwtGenerator    │
└─────────────────────────────────────────────────────────┘


#Beneficios de la Arquitectura
1) Independencia del Framework: El dominio no depende de tecnologías específicas
2) Testeable: Cada capa puede probarse de forma aislada
3) Mantenible: Cambios en infraestructura no afectan la lógica de negocio
4) Flexible: Fácil cambiar implementaciones (ej: de in-memory a SQL)