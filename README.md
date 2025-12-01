
# 🏥 CliniPlus — Sistema Integral de Gestión Médica  
Aplicación móvil y API para gestión de turnos, historia clínica, pacientes, médicos y administración del centro médico.

---
```

🧑‍💻 Autor
Gustavo Torres
Proyecto final - para la Materia de Programacion II - Aplicaciones Moviles con el Profesor Federico Trani - Tecnicatura en Desarrollo de Software – ITES Santa Rosa 2025
```
---
```
📝 Licencia

Este proyecto es de uso académico y privado. No está permitido su uso comercial.
```
---
```
⭐ Estado del Proyecto

🚧 En desarrollo activo
Todos los módulos principales están funcionando y validados.
```
---
## 📌 Descripción del Proyecto

CliniPlus es un sistema completo para clínicas y consultorios que permite:

- Gestión de **turnos médicos** (pacientes, secretaría y médicos)
- Historia clínica y registro de consultas
- Agenda diaria para médicos
- Gestión de pacientes (alta, provisionales, activación de cuentas)
- Gestión de médicos, especialidades y horarios
- Ficha médica del paciente (grupo sanguíneo, alergias, antecedentes, medicación)
- Inicio de sesión seguro con **JWT**
- Roles: **Administrador**, **Secretaría**, **Médico**, **Paciente**

El proyecto está dividido en:
- **CliniPlus.Api** → Backend ASP.NET Core Web API + EF Core 
- **CliniPlus.Movil** → App móvil con .NET MAUI Blazor Hybrid  
- **CliniPlus.Shared** → DTOs y Modelos (BD)

---
## ⚙️ Tecnologías Utilizadas

### **Frontend — .NET MAUI Blazor**
- .NET 8
- Blazor Hybrid
- Bootstrap Icons
- CSS customizado estilo mobile
- Navegación por rutas
- Inyección de dependencias

### **Backend — ASP.NET Core Web API**
- JWT Authentication
- Entity Framework Core
- SQL Server
- LINQ + Fluent API
- Repository Pattern
- Validaciones automáticas y manuales

### **Base de Datos**
- SQL Server / Monster ASP.NET
- Migraciones con EF Core

---

## 🔐 Seguridad

Todos los endpoints están protegidos con **JWT Bearer Authentication**, excepto:
- Login
- Registro público de turno provisional
- Consulta de slots públicos

Roles soportados:
- `Administrador`
- `Secretaria`
- `Medico`
- `Paciente`

Políticas:
- Política `SecretariaOAdministrador` para permitir accesos combinados.

---

## 📱 Funcionalidades por Rol

### 🧑‍⚕️ **Médico**
- Ver turnos del día
- Ver detalles del turno
- Completar consulta médica
- Registrar evolución
- Ver pacientes propios
- Ver historia clínica filtrada
- Ver ficha médica del paciente

---

### 👩‍💼 **Secretaría**
- Listado de pacientes
- Alta paciente
- Activación de pacientes provisionales
- Gestión rápida de turnos del día
- Agenda por médico
- Cancelación de turnos
- Visualizar historia clínica
- Visualizar ficha médica

---

### 👤 **Paciente**
- Ver mis turnos
- Cancelar turnos
- Reservar turnos
- Ver historia clínica personal

---

### 🛠️ **Administrador**
- Gestión completa (pacientes, médicos, obras sociales, especialidades, horarios)

---

## 🚀 Instalación

### 1. Clonar repositorio
```bash
git clone https://github.com/tu-repo/cliniPlus.git
```
---
⭐ Estado del Proyecto

🚧 En desarrollo activo
Todos los módulos principales están funcionando y validados.

