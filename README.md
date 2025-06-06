# 🪑 FurniTour - Платформа для замовлення меблів

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-18.2-red.svg)](https://angular.io/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-blue.svg)](https://www.microsoft.com/sql-server/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple.svg)](https://getbootstrap.com/)

## 📝 Опис проекту

FurniTour - це веб-платформа для замовлення індивідуальних меблів з можливостями чату в реальному часі, відстеження замовлень та штучного інтелекту для консультацій. Платформа об'єднує клієнтів з майстрами-меблярами для создания унікальних меблевих рішень.

### 🎯 Основні функції

- 👤 **Аутентифікація користувачів** - реєстрація, логін, ролі (User, Master, Admin)
- 💬 **Чат в реальному часі** - SignalR для миттєвого спілкування
- 📋 **Система замовлень** - створення, відстеження, управління статусами
- 🤖 **AI асистент** - інтеграція з Groq AI для консультацій
- 🛡️ **Система гарантій** - управління гарантійними зобов'язаннями
- 📊 **Адміністративна панель** - управління користувачами та замовленнями
- 📱 **Адаптивний дизайн** - Bootstrap 5 для всіх пристроїв

## 🏗️ Архітектура

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Angular 18    │◄──►│  ASP.NET Core    │◄──►│  SQL Server     │
│   Frontend      │    │    Backend       │    │   Database      │
│                 │    │                  │    │                 │
│ • Components    │    │ • Web API        │    │ • Entity        │
│ • Services      │    │ • SignalR Hubs   │    │   Framework     │
│ • Guards        │    │ • JWT Auth       │    │ • LocalDB       │
│ • Interceptors  │    │ • Groq AI        │    │ • Migrations    │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

## ⚙️ Технології

### Backend (.NET 8)
- **ASP.NET Core** - Web API та MVC
- **Entity Framework Core** - ORM для роботи з БД
- **SQL Server LocalDB** - база даних
- **SignalR** - real-time комунікація
- **JWT Bearer** - автентифікація та авторизація
- **Groq AI SDK** - інтеграція штучного інтелекту

### Frontend (Angular 18)
- **Angular CLI** - розробка та збірка
- **TypeScript** - типізована розробка
- **RxJS** - реактивне програмування
- **Bootstrap 5** - UI фреймворк
- **SignalR Client** - real-time з'єднання
- **Angular HTTP Client** - API комунікація

## 🚀 Встановлення та запуск

### Передумови
- **Node.js**: ^18.19.1 || ^20.11.1 || ^22.0.0
- **npm**: >=6.5.3 || >=7.4.0
- **.NET 8 SDK**
- **SQL Server Express LocalDB**
- **Angular CLI**: `npm install -g @angular/cli`

### Кроки встановлення

1. **Клонування репозиторію**
```bash
git clone <repository-url>
cd FurniTour
```

2. **Налаштування Backend**
```bash
cd FurniTour.Server
dotnet restore
dotnet ef database update
dotnet run
```

3. **Налаштування Frontend**
```bash
cd furnitour.client
npm install
ng serve
```

4. **Доступ до застосунку**
- Frontend: http://localhost:4200
- Backend API: https://localhost:7043

### Конфігурація

#### Backend конфігурація (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FurniTourDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "FurniTour",
    "Audience": "FurniTour"
  }
}
```

#### Frontend конфігурація (`environments/app.environment.ts`)
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7043/api',
  hubUrl: 'https://localhost:7043/chathub'
};
```

#### Groq AI конфігурація (`api.json`)
```json
{
  "groq": {
    "api_key": "your-groq-api-key"
  }
}
```

## 🧪 Проблеми і рішення

### Backend проблеми

| Проблема              | Рішення                            |
|----------------------|------------------------------------|
| 500 Internal Server Error | Перевірити Connection String до SQL Server та наявність бази даних |
| CORS помилка         | Налаштувати CORS policy у `Program.cs` для Angular додатку |
| Entity Framework помилки | Виконати `update-database` та перевірити міграції |
| SignalR підключення не працює | Перевірити конфігурацію Hubs у `Program.cs` та клієнтську частину |
| JWT токени не валідуються | Налаштувати JWT middleware з правильними ключами |
| Не зберігає файли зображень | Перевірити права доступу до папки wwwroot та налаштування статичних файлів |
| Groq API не відповідає | Перевірити валідність API ключа у `api.json` |
| SQL Server LocalDB не доступна | Встановити/переустановити SQL Server Express LocalDB |

### Frontend проблеми  

| Проблема              | Рішення                            |
|----------------------|------------------------------------|
| Angular додаток не запускається | Перевірити версію Node.js (^18.19.1 або ^20.11.1 або ^22.0.0) |
| HTTP Interceptor блокує запити | Перевірити конфігурацію у `app.interceptor.ts` |
| Роутинг не працює | Перевірити Guard-и у `app.auth.guard.ts` та маршрути |
| Bootstrap стилі не завантажуються | Виконати `npm install` та перевірити angular.json |
| SignalR клієнт не підключається | Перевірити URL хабу та конфігурацію у сервісах |
| Проксі до API не працює | Налаштувати proxy.conf.json для проксі до ASP.NET Core |
| TypeScript помилки компіляції | Оновити типи та перевірити tsconfig.json |
| Відсутні Angular CLI команди | Встановити Angular CLI глобально: `npm install -g @angular/cli` |

### База даних проблеми

| Проблема              | Рішення                            |
|----------------------|------------------------------------|
| Міграції не застосовуються | Виконати `dotnet ef database update` з папки Server |
| Помилки Foreign Key | Перевірити порядок додавання даних та зв'язки між таблицями |
| Повільні запити | Додати індекси для часто використовуваних полів |
| Connection timeout | Збільшити timeout у Connection String |
| Блокування транзакцій | Оптимізувати запити та використовувати async/await |

### Розгортання проблеми

| Проблема              | Рішення                            |
|----------------------|------------------------------------|
| IIS не обслуговує Angular | Налаштувати URL Rewrite для SPA маршрутизації |
| HTTPS сертифікати | Налаштувати SSL сертифікати для production |
| Environment змінні | Створити окремі appsettings для різних середовищ |
| Статичні файли не доступні | Налаштувати IIS для обслуговування статичного контенту |

## 🔧 Корисні команди

### Backend команди
```bash
# Створення міграції
dotnet ef migrations add MigrationName

# Застосування міграцій
dotnet ef database update

# Відкат міграції
dotnet ef database update PreviousMigrationName

# Запуск в Development режимі
dotnet run --environment Development
```

### Frontend команди
```bash
# Розробка з auto-reload
ng serve

# Збірка для production
ng build --prod

# Запуск тестів
ng test

# Лінтинг коду
ng lint

# Оновлення Angular
ng update @angular/cli @angular/core
```

## 📚 Структура проекту

```
FurniTour/
├── FurniTour.Server/              # ASP.NET Core Backend
│   ├── Controllers/               # API контролери
│   ├── Models/                    # Entity моделі
│   ├── Data/                      # DbContext та міграції
│   ├── Hubs/                      # SignalR хаби
│   ├── Services/                  # Бізнес логіка
│   └── Program.cs                 # Налаштування додатку
├── furnitour.client/              # Angular Frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/        # Angular компоненти
│   │   │   ├── services/          # Angular сервіси
│   │   │   ├── guards/            # Route guards
│   │   │   ├── interceptors/      # HTTP interceptors
│   │   │   └── models/            # TypeScript моделі
│   │   ├── assets/                # Статичні ресурси
│   │   └── environments/          # Конфігурація середовищ
├── ІНСТРУКЦІЯ_КОРИСТУВАЧА.md      # Детальна інструкція користувача
└── README.md                      # Документація проекту
```

## 📖 Використовані джерела та література

### Офіційна документація
1. **Microsoft .NET Documentation** - https://docs.microsoft.com/dotnet/
   - ASP.NET Core Web API Development
   - Entity Framework Core Guide
   - SignalR Real-time Communication

2. **Angular Documentation** - https://angular.io/docs
   - Angular Architecture and Components
   - HTTP Client and Services
   - Reactive Forms and Routing

3. **SQL Server Documentation** - https://docs.microsoft.com/sql/
   - LocalDB Configuration and Management
   - T-SQL Query Optimization
   - Database Design Best Practices

### Технічні ресурси
4. **Bootstrap Documentation** - https://getbootstrap.com/docs/5.3/
   - Responsive Design Components
   - CSS Grid and Flexbox Systems

5. **SignalR Documentation** - https://docs.microsoft.com/aspnet/signalr/
   - Real-time Web Applications
   - Hub Configuration and Clients

6. **JWT.io** - https://jwt.io/introduction/
   - JSON Web Token Implementation
   - Security Best Practices

### Розробка та тестування
7. **Angular CLI Documentation** - https://angular.io/cli
   - Project Generation and Building
   - Testing and Linting Tools

8. **Entity Framework Core Documentation** - https://docs.microsoft.com/ef/core/
   - Code-First Migrations
   - LINQ to Entities Queries

### AI та інтеграції  
9. **Groq AI Documentation** - https://console.groq.com/docs/
   - API Integration Guide
   - Natural Language Processing

10. **TypeScript Handbook** - https://www.typescriptlang.org/docs/
    - Type System and Interfaces
    - Advanced TypeScript Features

### Книги та посібники
11. **"Pro ASP.NET Core 6"** by Adam Freeman
    - Advanced Web API Development
    - Dependency Injection Patterns

12. **"Angular Development with TypeScript"** by Yakov Fain, Anton Moiseev
    - Component Architecture
    - State Management Patterns

## 🤝 Внесок у проект

1. Створіть Fork репозиторію
2. Створіть гілку для нової функції (`git checkout -b feature/amazing-feature`)
3. Зафіксуйте зміни (`git commit -m 'Add amazing feature'`)
4. Завантажте гілку (`git push origin feature/amazing-feature`)
5. Створіть Pull Request

## 📄 Ліцензія

Цей проект ліцензовано під MIT License - див. файл [LICENSE](LICENSE) для деталей.

## 📞 Контакти

- **Розробник**: [Ваше ім'я]
- **Email**: [ваш-email@example.com]
- **Проект**: [посилання-на-репозиторій]

---

> 💡 **Порада**: Для детальних інструкцій користувача дивіться файл [ІНСТРУКЦІЯ_КОРИСТУВАЧА.md](ІНСТРУКЦІЯ_КОРИСТУВАЧА.md)