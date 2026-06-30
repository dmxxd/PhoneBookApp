# 📱 PhoneBookApp

Настольное приложение для хранения и управления телефонной книгой с поддержкой авторизации пользователей и административной панели.

---

# Возможности

## Пользователь

1. Регистрация и вход в систему;
2. Просмотр списка контактов;
3. Добавление новых контактов;
4. Редактирование существующих;
5. Удаление контактов;
6. Поиск по имени, телефону и электронной почте;
7. Просмотр полной информации о контакте;
8. Хранение даты рождения и заметок.

## Администратор
1. Просмотр пользователей;
2. Создание новых пользователей;
3. Изменение ролей;
4. Блокировка и удаление пользователей.

---

# Руководство пользователя

## Авторизация

После запуска приложения открывается окно входа.

Введите электронную почту и пароль, затем нажмите кнопку **«Войти»**.

Если учетной записи нет, перейдите к регистрации.

<p align="center">
<img src="images/login.jpg" width="500">
</p>

---

## Регистрация

Для создания новой учетной записи необходимо заполнить регистрационную форму и подтвердить создание аккаунта.

<p align="center">
<img src="images/registration.jpg" width="500">
</p>

---

## Главное окно

После успешной авторизации открывается главное окно приложения.

Здесь пользователь может:

1. Просматривать контакты;
2. Выполнять поиск;
3. Добавлять новые записи;
5. Редактировать существующие контакты;
6. Удалять контакты.

<p align="center">
<img src="images/main.jpg" width="700">
</p>

---

## Контакт

При выборе контакта открывается окно с подробной информацией.

Доступны следующие действия:

1. Просмотр информации;
2. Изменение данных;
3. Удаление контакта;
4. Сохранение изменений.

<p align="center">
<img src="images/contact.jpg" width="500">
</p>

---

## Управление пользователями

Данный раздел доступен только администратору.

Здесь можно:

1. Просматривать список пользователей;
2. Создавать новые учетные записи;
3. Изменять права доступа;
4. Удалять пользователей.

<p align="center">
<img src="images/user_management.jpg" width="700">
</p>

---

# Использование приложения

1. Запустите программу.
2. Зарегистрируйтесь или выполните вход.
3. Добавьте новый контакт.
4. Используйте поиск для быстрого нахождения нужной записи.
5. При необходимости измените или удалите контакт.
6. Если вы являетесь администратором, воспользуйтесь разделом управления пользователями.

---

# Используемые технологии

1. C#
2. .NET
3. Windows Forms
4. SQLite

---

# Структура проекта

```text
PhoneBookApp/
│
├── PhoneBookApp.sln              # Решение Visual Studio
├── README.md                     # Главная документация
│
├── docs/                         # Документация проекта
│   ├── diagrams.md               # Диаграммы
│   ├── mockups.md                # Макеты интерфейса
│   └── requirements.md           # Требования к системе
│
├── images/                       # Медиафайлы (скриншоты, макеты, диаграммы)
│   ├── login.jpg
│   ├── registration.jpg
│   ├── main.jpg
│   ├── contact.jpg
│   ├── user_management.jpg
│   ├── inf_model.png
│   ├── er_diagram.png
│   ├── use_case_diagram.png
│   ├── class_diagram.png
│   ├── sequence_diagram.png
│   ├── component_diagram.png
│   ├── program_graph.png
│   ├── login_mockup
│   ├── registration_mockup
│   ├── main_mockup
│   ├── contact_mockup
│   └── user_management_mockup
│
├── PhoneBookApp/                 # Исходный код приложения (Windows Forms)
│   ├── App.config                 # Конфигурация приложения
│   ├── packages.config            # Зависимости NuGet
│   ├── PhoneBookApp.csproj        # Файл проекта
│   ├── Program.cs                  # Точка входа в приложение
│   │
│   ├── Database/                  # Работа с базой данных SQLite
│   │   └── DatabaseHelper.cs
│   │
│   ├── Forms/                     # Графические окна (Windows Forms)
│   │   ├── ContactForm.cs
│   │   ├── ContactForm.Designer.cs
│   │   ├── ContactForm.resx
│   │   ├── LoginForm.cs
│   │   ├── LoginForm.Designer.cs
│   │   ├── LoginForm.resx
│   │   ├── MainForm.cs
│   │   ├── MainForm.Designer.cs
│   │   ├── MainForm.resx
│   │   ├── RegisterForm.cs
│   │   ├── RegisterForm.Designer.cs
│   │   ├── UserManagementForm.cs
│   │   ├── UserManagementForm.Designer.cs
│   │   └── UserManagementForm.resx
│   │
│   ├── Helpers/                   # Вспомогательные утилиты
│   │   ├── PasswordHelper.cs
│   │   └── ThemeManager.cs
│   │
│   ├── Models/                    # Модели данных (сущности)
│   │   ├── Contact.cs
│   │   └── User.cs
│   │
│   └── Properties/                # Настройки и ресурсы проекта
│       ├── AssemblyInfo.cs
│       ├── Resources.Designer.cs
│       ├── Resources.resx
│       ├── Settings.Designer.cs
│       └── Settings.settings
│
└── tests/                         # Тестирование проекта
    ├── integration_test.md        # Интеграционные тесты
    └── module_test.md             # Модульные тесты
```

---

# Требования

1. Windows 10/11;
2. .NET Desktop Runtime;
3. Visual Studio 2022-2026.

---

# Автор

Мальков Д.Д.
