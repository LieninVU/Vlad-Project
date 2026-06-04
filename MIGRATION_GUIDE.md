# Руководство по миграции базы данных

## 📋 Проблема

При запуске приложения возникает ошибка:
```
Ошибка подключения к базе данных
Недопустимое имя столбца "PaymentScheduleType"
```

Это означает, что ваша база данных была создана по старым SQL скриптам и не содержит нового столбца `PaymentScheduleType` в таблице `Contracts`.

## ✅ Решения

### 🔄 Вариант 1: Выполнить миграционный скрипт (рекомендуется)

Если у вас уже есть база данных с данными и вы не хотите ее терять:

1. Откройте **SQL Server Management Studio**
2. Подключитесь к вашему серверу SQL Server
3. Выберите базу данных **LeasingSystem**
4. Выполните скрипт:
   ```sql
   -- Database/06_Migration_PaymentScheduleType.sql
   USE LeasingSystem;
   GO
   
   -- Добавляем поле PaymentScheduleType в Contracts, если его нет
   IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'PaymentScheduleType')
   BEGIN
       ALTER TABLE Contracts ADD PaymentScheduleType TINYINT NULL DEFAULT 1;
       PRINT 'Поле PaymentScheduleType добавлено в таблицу Contracts.';
   END
   ELSE
   BEGIN
       PRINT 'Поле PaymentScheduleType уже существует.';
   END
   GO
   
   -- Обновляем существующие записи
   UPDATE Contracts SET PaymentScheduleType = 1 WHERE PaymentScheduleType IS NULL;
   GO
   
   -- Добавляем CHECK-ограничение
   IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Contracts_PaymentScheduleType')
   BEGIN
       ALTER TABLE Contracts ADD CONSTRAINT CK_Contracts_PaymentScheduleType 
           CHECK (PaymentScheduleType BETWEEN 0 AND 2);
       PRINT 'Ограничение CK_Contracts_PaymentScheduleType добавлено.';
   END
   GO
   ```

5. Запустите приложение заново

---

### 🆕 Вариант 2: Создать новую базу данных (если данных нет или можно потерять)

Если у вас нет важных данных или вы только начинаете:

1. Удалите старую базу данных **LeasingSystem** через SSMS
2. Выполните все SQL скрипты из папки **Database** в следующем порядке:
   
   | № | Файл | Описание |
   |---|------|----------|
   | 1 | 01_CreateDatabase.sql | Создание базы данных LeasingSystem |
   | 2 | 02_CreateTables.sql | Создание всех таблиц (включая PaymentScheduleType) |
   | 3 | 03_CreateConstraints.sql | Создание ограничений |
   | 4 | 04_CreateStoredProcedures.sql | Создание хранимых процедур |
   | 5 | 05_SeedData.sql | Заполнение демонстрационными данными |
   | 6 | 06_Migration_PaymentScheduleType.sql | Миграция для PaymentScheduleType |
   | 7 | 07_CreateUsersTable.sql | Создание таблицы Users для аутентификации |

3. Запустите приложение

---

### 🔧 Вариант 3: Обновить существующие скрипты

Если вы хотите обновить свои SQL скрипты для будущих установок:

1. Откройте файл **Database/02_CreateTables.sql**
2. Убедитесь, что в таблице Contracts есть строка:
   ```sql
   PaymentScheduleType TINYINT NOT NULL DEFAULT 1,
   ```
3. Добавьте ограничение:
   ```sql
   ALTER TABLE Contracts ADD CONSTRAINT CK_Contracts_PaymentScheduleType CHECK (PaymentScheduleType BETWEEN 0 AND 2);
   ```

---

## 📊 Что изменилось

### Новые столбцы
| Таблица | Столбец | Тип | Описание | Значение по умолчанию |
|---------|---------|-----|----------|---------------------|
| Contracts | PaymentScheduleType | TINYINT | Тип графика платежей | 1 (Monthly) |

### Новые таблицы
| Таблица | Описание |
|---------|----------|
| Users | Хранение пользователей системы для Windows аутентификации |

---

## 🎯 Дополнительные действия

После миграции:

1. **Проверьте данные**: Убедитесь, что все существующие договоры отображаются корректно
2. **Проверьте графики платежей**: При создании/редактировании договора выберите тип графика платежей (OneTime, Monthly, Custom)
3. **Проверьте аутентификацию**: При первом запуске текущий пользователь Windows будет добавлен в таблицу Users

---

## ❓ Вопросы и ответы

**В: Можно ли выполнить миграцию на работающей системе?**  
О: Да, миграционный скрипт 06_Migration_PaymentScheduleType.sql добавлет столбец с значением по умолчанию, поэтому существующие данные не будут потеряны.

**В: Что делать, если после миграции приложение все равно не запускается?**  
О: Убедитесь, что:
- SQL Server Express запущен
- Вы подключены к правильному серверу
- База данных LeasingSystem существует
- Вы выполнили все необходимые скрипты

**В: Как проверить, что столбец PaymentScheduleType добавлен?**   
О: Выполните запрос в SSMS:
```sql
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Contracts' AND COLUMN_NAME = 'PaymentScheduleType';
```
Если запрос возвращает строку, то столбец существует.

---

## 🆘 Техническая поддержка

Если у вас возникли проблемы:
1. Проверьте journal ошибок SQL Server
2. Проверьте сообщение об ошибке в приложении
3. Убедитесь, что у вас есть резервная копия базы данных перед выполнением миграции

---

**© 2026 LeasingSystem**
