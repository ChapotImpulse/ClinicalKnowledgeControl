-- Создание базы данных
CREATE DATABASE ClinicalControl;
GO

USE ClinicalControl;
GO

-- Справочник ролей
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200)
);
GO

-- Справочник отделений
CREATE TABLE Departments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT DEFAULT 1
);
GO

-- Сотрудники (врачи, заведующие, администраторы)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(200) NOT NULL,
    DepartmentId INT NULL,
    RoleId INT NOT NULL,
    Specialty NVARCHAR(100),
    Login NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Departments FOREIGN KEY (DepartmentId) REFERENCES Departments(Id),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO

-- Клинические рекомендации (КР)
CREATE TABLE ClinicalGuidelines (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(300) NOT NULL,
    ICDCode NVARCHAR(20),
    UpdateDate DATE,
    EffectiveDate DATE,
    FileLink NVARCHAR(500),
    Description NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Банк вопросов
CREATE TABLE Questions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ClinicalGuidelineId INT NOT NULL,
    Text NVARCHAR(MAX) NOT NULL,
    QuestionType INT NOT NULL, -- 1-одиночный, 2-множественный
    Tags NVARCHAR(200),
    Explanation NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedBy INT,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedBy INT NULL,
	UpdatedDate DATETIME NULL,
    CONSTRAINT FK_Questions_ClinicalGuidelines FOREIGN KEY (ClinicalGuidelineId) REFERENCES ClinicalGuidelines(Id),
    CONSTRAINT FK_Questions_Users FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    CONSTRAINT FK_Questions_Users_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
);
GO

-- Варианты ответов
CREATE TABLE QuestionOptions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    QuestionId INT NOT NULL,
    Text NVARCHAR(MAX) NOT NULL,
    IsCorrect BIT DEFAULT 0,
    SequenceOrder INT NULL, -- Для вопросов типа "последовательность"
    CONSTRAINT FK_QuestionOptions_Questions FOREIGN KEY (QuestionId) REFERENCES Questions(Id) ON DELETE CASCADE
);
GO

-- Шаблоны тестов
CREATE TABLE TestTemplates (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    ClinicalGuidelineId INT NOT NULL,
    QuestionCount INT NOT NULL,
    TimeLimitMinutes INT NOT NULL,
    PassingScore DECIMAL(5,2) NOT NULL, -- Процент (например, 70.00)
    MaxAttempts INT DEFAULT 2,
    IsActive BIT DEFAULT 1,
    CreatedBy INT,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedBy INT NULL,
	UpdatedDate DATETIME NULL,
    CONSTRAINT FK_TestTemplates_ClinicalGuidelines FOREIGN KEY (ClinicalGuidelineId) REFERENCES ClinicalGuidelines(Id),
    CONSTRAINT FK_TestTemplates_Users FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    CONSTRAINT FK_TestTemplates_Users_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
);
GO

-- Назначения тестов
CREATE TABLE TestAssignments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TestTemplateId INT NOT NULL,
    TargetType INT NOT NULL, -- 1-всё отделение, 2-специальность, 3-конкретный врач
    TargetId INT NOT NULL, -- DepartmentId, Specialty или UserId
    Deadline DATE NOT NULL,
    IsAutoAssigned BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive [bit] NULL,
	UpdatedDate [datetime] NULL,
    CONSTRAINT FK_TestAssignments_TestTemplates FOREIGN KEY (TestTemplateId) REFERENCES TestTemplates(Id)
);
GO

-- Попытки прохождения тестов
CREATE TABLE TestAttempts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    TestTemplateId INT NOT NULL,
    AssignmentId INT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NULL,
    Score DECIMAL(5,2) NULL, -- Процент правильных ответов
    Status INT NOT NULL, -- 1-в процессе, 2-завершен, 3-сдан, 4-не сдан, 5-время вышло
    IsPassed BIT NULL,
    CONSTRAINT FK_TestAttempts_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_TestAttempts_TestTemplates FOREIGN KEY (TestTemplateId) REFERENCES TestTemplates(Id),
    CONSTRAINT FK_TestAttempts_TestAssignments FOREIGN KEY (AssignmentId) REFERENCES TestAssignments(Id)
);
GO

-- Ответы в рамках попытки
CREATE TABLE AttemptAnswers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AttemptId INT NOT NULL,
    QuestionId INT NOT NULL,
    SelectedOptions NVARCHAR(MAX), -- JSON или CSV с ID выбранных вариантов
    IsCorrect BIT NULL,
    TimeSpentSeconds INT,
    CONSTRAINT FK_AttemptAnswers_TestAttempts FOREIGN KEY (AttemptId) REFERENCES TestAttempts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AttemptAnswers_Questions FOREIGN KEY (QuestionId) REFERENCES Questions(Id)
);
GO

-- Индексы для производительности
CREATE INDEX IX_Users_DepartmentId ON Users(DepartmentId);
CREATE INDEX IX_Users_RoleId ON Users(RoleId);
CREATE INDEX IX_Questions_ClinicalGuidelineId ON Questions(ClinicalGuidelineId);
CREATE INDEX IX_TestAttempts_UserId ON TestAttempts(UserId);
CREATE INDEX IX_TestAttempts_TestTemplateId ON TestAttempts(TestTemplateId);
CREATE INDEX IX_TestAttempts_StartTime ON TestAttempts(StartTime);
CREATE INDEX IX_AttemptAnswers_AttemptId ON AttemptAnswers(AttemptId);
GO

-- Роли
INSERT INTO Roles (Name, Description) VALUES
('Врач', 'Проходит тестирование'),
('Заведующий отделением', 'Просматривает отчеты по отделению'),
('Заместитель главного врача', 'Просматривает отчеты по учреждению'),
('Администратор', 'Управляет системой'),
('ГВС', 'Разрабатывает тесты');
GO

-- Пример отделения
INSERT INTO Departments (Name) VALUES ('Терапевтическое отделение');
INSERT INTO Departments (Name) VALUES ('Кардиологическое отделение');
INSERT INTO Departments (Name) VALUES ('Администраторы');
GO

-- Тестовые пользователи
INSERT Users (FullName, DepartmentId, RoleId, Specialty, Login, PasswordHash, IsActive) 
VALUES (N'Администратор', 3, 4, N'Администратор', N'admin', N'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', 1)
GO
INSERT Users (FullName, DepartmentId, RoleId, Specialty, Login, PasswordHash, IsActive)
VALUES (2, N'Иванов И.И.', 1, 1, N'Кардиология', N'ivanov_ii', N'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', 1)
GO

-- Процедура для получения случайных вопросов по шаблону теста
CREATE PROCEDURE sp_GetRandomQuestionsForTest
    @TestTemplateId INT,
    @QuestionCount INT
AS
BEGIN
    SELECT TOP (@QuestionCount)
        q.Id,
        q.Text,
        q.QuestionType,
        q.Explanation
    FROM Questions q
    INNER JOIN TestTemplates t ON q.ClinicalGuidelineId = t.ClinicalGuidelineId
    WHERE t.Id = @TestTemplateId
      AND q.IsActive = 1
    ORDER BY NEWID();
END
GO

-- Процедура для проверки возможности начать попытку
CREATE PROCEDURE sp_CanStartAttempt
    @UserId INT,
    @TestTemplateId INT,
    @MaxAttempts INT,
    @CanStart BIT OUTPUT,
    @AttemptsToday INT OUTPUT
AS
BEGIN
    SELECT @AttemptsToday = COUNT(*)
    FROM TestAttempts
    WHERE UserId = @UserId
      AND TestTemplateId = @TestTemplateId
      AND CAST(StartTime AS DATE) = CAST(GETDATE() AS DATE);

    IF @AttemptsToday >= @MaxAttempts
        SET @CanStart = 0;
    ELSE
        SET @CanStart = 1;
END
GO

-- Процедура для получения сводки по отделению
CREATE PROCEDURE sp_GetDepartmentReport
    @DepartmentId INT
AS
BEGIN
    SELECT
        u.FullName AS DoctorName,
        u.Specialty,
        tt.Name AS TestName,
        ta.Deadline,
        CASE
            WHEN MAX(ta.Id) IS NULL THEN 'Не назначено'
            WHEN MAX(CASE WHEN att.Status = 3 THEN 1 ELSE 0 END) = 1 THEN 'Сдано'
            WHEN MAX(CASE WHEN att.Status = 4 THEN 1 ELSE 0 END) = 1 THEN 'Не сдано'
            WHEN ta.Deadline < GETDATE() THEN 'Просрочено'
            ELSE 'Ожидает прохождения'
        END AS Status,
        MAX(att.Score) AS Score,
        MAX(att.EndTime) AS CompletionDate
    FROM Users u
    LEFT JOIN TestAssignments ta ON
        (ta.TargetType = 1 AND ta.TargetId = u.DepartmentId) OR
        (ta.TargetType = 2 AND ta.TargetId = u.Specialty) OR
        (ta.TargetType = 3 AND ta.TargetId = u.Id)
    LEFT JOIN TestTemplates tt ON ta.TestTemplateId = tt.Id
    LEFT JOIN TestAttempts att ON ta.Id = att.AssignmentId AND att.UserId = u.Id
    WHERE u.DepartmentId = @DepartmentId
      AND u.RoleId = 1 -- Только врачи
    GROUP BY u.FullName, u.Specialty, tt.Name, ta.Deadline
    ORDER BY u.FullName, tt.Name;
END
GO

-- Процедура для автоматического назначения тестов при вступлении в силу новой КР
CREATE PROCEDURE sp_AutoAssignNewGuidelines
AS
BEGIN
    INSERT INTO TestAssignments (TestTemplateId, TargetType, TargetId, Deadline, IsAutoAssigned)
    SELECT
        tt.Id,
        2, -- По специальности
        cg.Id, -- В данном примере TargetId = ClinicalGuidelineId (упрощенно)
        DATEADD(DAY, 30, GETDATE()), -- Дедлайн через 30 дней
        1
    FROM ClinicalGuidelines cg
    INNER JOIN TestTemplates tt ON cg.Id = tt.ClinicalGuidelineId
    WHERE cg.EffectiveDate <= GETDATE()
      AND cg.Id NOT IN (
          SELECT DISTINCT tt2.ClinicalGuidelineId
          FROM TestAssignments ta2
          INNER JOIN TestTemplates tt2 ON ta2.TestTemplateId = tt2.Id
          WHERE ta2.IsAutoAssigned = 1
      );
END
GO