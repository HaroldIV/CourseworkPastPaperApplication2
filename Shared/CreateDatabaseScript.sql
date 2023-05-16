CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE TABLE public."Students" (
        "Id" uuid NOT NULL,
        "Name" text NOT NULL,
        "Password" bytea NOT NULL,
        CONSTRAINT "PK_Students" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE TABLE public."Teachers" (
        "Id" uuid NOT NULL,
        "Name" text NOT NULL,
        "Password" bytea NOT NULL,
        CONSTRAINT "PK_Teachers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE TABLE public."Assignments" (
        "Id" uuid NOT NULL,
        "Set" timestamp with time zone NOT NULL,
        "Due" timestamp with time zone NOT NULL,
        "StudentId" uuid NULL,
        CONSTRAINT "PK_Assignments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Assignments_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES public."Students" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE TABLE public."Classes" (
        "Id" uuid NOT NULL,
        "TeacherId" uuid NOT NULL,
        CONSTRAINT "PK_Classes" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Classes_Teachers_TeacherId" FOREIGN KEY ("TeacherId") REFERENCES public."Teachers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE TABLE public."PaperResults" (
        "Id" uuid NOT NULL,
        "Score" integer NOT NULL,
        "AssignmentId" uuid NOT NULL,
        "StudentId" uuid NULL,
        CONSTRAINT "PK_PaperResults" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PaperResults_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_PaperResults_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES public."Students" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE TABLE public."Questions" (
        "Id" uuid NOT NULL,
        "Data" bytea NOT NULL,
        "ReadData" text NOT NULL,
        "AssignmentId" uuid NOT NULL,
        CONSTRAINT "PK_Questions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Questions_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE TABLE public."ClassStudent" (
        "CurrentClassesId" uuid NOT NULL,
        "StudentsId" uuid NOT NULL,
        CONSTRAINT "PK_ClassStudent" PRIMARY KEY ("CurrentClassesId", "StudentsId"),
        CONSTRAINT "FK_ClassStudent_Classes_CurrentClassesId" FOREIGN KEY ("CurrentClassesId") REFERENCES public."Classes" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ClassStudent_Students_StudentsId" FOREIGN KEY ("StudentsId") REFERENCES public."Students" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_Assignments_Id" ON public."Assignments" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_Assignments_StudentId" ON public."Assignments" ("StudentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_Classes_Id" ON public."Classes" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_Classes_TeacherId" ON public."Classes" ("TeacherId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_ClassStudent_StudentsId" ON public."ClassStudent" ("StudentsId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_PaperResults_AssignmentId" ON public."PaperResults" ("AssignmentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_PaperResults_Id" ON public."PaperResults" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_PaperResults_StudentId" ON public."PaperResults" ("StudentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_Questions_AssignmentId" ON public."Questions" ("AssignmentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_Questions_Id" ON public."Questions" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_Students_Id" ON public."Students" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    CREATE INDEX "IX_Teachers_Id" ON public."Teachers" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230216011619_One') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230216011619_One', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230305202500_AssignmentsQuestionsIntoManyToMany') THEN
    ALTER TABLE public."Questions" DROP CONSTRAINT "FK_Questions_Assignments_AssignmentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230305202500_AssignmentsQuestionsIntoManyToMany') THEN
    DROP INDEX public."IX_Questions_AssignmentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230305202500_AssignmentsQuestionsIntoManyToMany') THEN
    ALTER TABLE public."Questions" DROP COLUMN "AssignmentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230305202500_AssignmentsQuestionsIntoManyToMany') THEN
    CREATE TABLE public."AssignmentQuestion" (
        "AssignmentId" uuid NOT NULL,
        "QuestionsId" uuid NOT NULL,
        CONSTRAINT "PK_AssignmentQuestion" PRIMARY KEY ("AssignmentId", "QuestionsId"),
        CONSTRAINT "FK_AssignmentQuestion_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_AssignmentQuestion_Questions_QuestionsId" FOREIGN KEY ("QuestionsId") REFERENCES public."Questions" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230305202500_AssignmentsQuestionsIntoManyToMany') THEN
    CREATE INDEX "IX_AssignmentQuestion_QuestionsId" ON public."AssignmentQuestion" ("QuestionsId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230305202500_AssignmentsQuestionsIntoManyToMany') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230305202500_AssignmentsQuestionsIntoManyToMany', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230305220645_AddedLevelExamBoardToQuestion') THEN
    ALTER TABLE public."Questions" ADD "ExamBoard" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230305220645_AddedLevelExamBoardToQuestion') THEN
    ALTER TABLE public."Questions" ADD "Level" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230305220645_AddedLevelExamBoardToQuestion') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230305220645_AddedLevelExamBoardToQuestion', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    ALTER TABLE public."Questions" DROP COLUMN "ExamBoard";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    ALTER TABLE public."Questions" DROP COLUMN "Level";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    ALTER TABLE public."Questions" ADD "ExamBoardId" smallint NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    ALTER TABLE public."Questions" ADD "LevelId" smallint NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    CREATE TABLE public."ValidExamBoards" (
        "Id" smallint GENERATED BY DEFAULT AS IDENTITY,
        "Name" text NOT NULL,
        CONSTRAINT "PK_ValidExamBoards" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    CREATE TABLE public."ValidLevels" (
        "Id" smallint GENERATED BY DEFAULT AS IDENTITY,
        "Name" text NOT NULL,
        CONSTRAINT "PK_ValidLevels" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    CREATE INDEX "IX_Questions_ExamBoardId" ON public."Questions" ("ExamBoardId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    CREATE INDEX "IX_Questions_LevelId" ON public."Questions" ("LevelId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    CREATE INDEX "IX_ValidExamBoards_Id" ON public."ValidExamBoards" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    CREATE INDEX "IX_ValidLevels_Id" ON public."ValidLevels" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    ALTER TABLE public."Questions" ADD CONSTRAINT "FK_Questions_ValidExamBoards_ExamBoardId" FOREIGN KEY ("ExamBoardId") REFERENCES public."ValidExamBoards" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    ALTER TABLE public."Questions" ADD CONSTRAINT "FK_Questions_ValidLevels_LevelId" FOREIGN KEY ("LevelId") REFERENCES public."ValidLevels" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230307110109_AddedFilterOptionsEntities') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230307110109_AddedFilterOptionsEntities', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."Questions" DROP CONSTRAINT "FK_Questions_ValidExamBoards_ExamBoardId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."Questions" DROP CONSTRAINT "FK_Questions_ValidLevels_LevelId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    DROP TABLE public."AssignmentQuestion";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."ValidLevels" DROP CONSTRAINT "PK_ValidLevels";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    DROP INDEX public."IX_ValidLevels_Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."ValidExamBoards" DROP CONSTRAINT "PK_ValidExamBoards";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    DROP INDEX public."IX_ValidExamBoards_Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."ValidLevels" RENAME TO "Level";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."ValidExamBoards" RENAME TO "ExamBoard";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    CREATE EXTENSION IF NOT EXISTS fuzzystrmatch;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."Questions" ADD "AssignmentId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."Level" ADD CONSTRAINT "PK_Level" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."ExamBoard" ADD CONSTRAINT "PK_ExamBoard" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    CREATE INDEX "IX_Questions_AssignmentId" ON public."Questions" ("AssignmentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."Questions" ADD CONSTRAINT "FK_Questions_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."Questions" ADD CONSTRAINT "FK_Questions_ExamBoard_ExamBoardId" FOREIGN KEY ("ExamBoardId") REFERENCES public."ExamBoard" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    ALTER TABLE public."Questions" ADD CONSTRAINT "FK_Questions_Level_LevelId" FOREIGN KEY ("LevelId") REFERENCES public."Level" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420210043_AddedLevelAndExamBoard') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230420210043_AddedLevelAndExamBoard', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421130101_AlteredLevelAndExamBoardOnQuestionToMakeThemOptional') THEN
    ALTER TABLE public."Questions" DROP CONSTRAINT "FK_Questions_ExamBoard_ExamBoardId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421130101_AlteredLevelAndExamBoardOnQuestionToMakeThemOptional') THEN
    ALTER TABLE public."Questions" DROP CONSTRAINT "FK_Questions_Level_LevelId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421130101_AlteredLevelAndExamBoardOnQuestionToMakeThemOptional') THEN
    ALTER TABLE public."Questions" ALTER COLUMN "LevelId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421130101_AlteredLevelAndExamBoardOnQuestionToMakeThemOptional') THEN
    ALTER TABLE public."Questions" ALTER COLUMN "ExamBoardId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421130101_AlteredLevelAndExamBoardOnQuestionToMakeThemOptional') THEN
    ALTER TABLE public."Questions" ADD CONSTRAINT "FK_Questions_ExamBoard_ExamBoardId" FOREIGN KEY ("ExamBoardId") REFERENCES public."ExamBoard" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421130101_AlteredLevelAndExamBoardOnQuestionToMakeThemOptional') THEN
    ALTER TABLE public."Questions" ADD CONSTRAINT "FK_Questions_Level_LevelId" FOREIGN KEY ("LevelId") REFERENCES public."Level" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421130101_AlteredLevelAndExamBoardOnQuestionToMakeThemOptional') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230421130101_AlteredLevelAndExamBoardOnQuestionToMakeThemOptional', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."Questions" DROP CONSTRAINT "FK_Questions_ExamBoard_ExamBoardId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."Questions" DROP CONSTRAINT "FK_Questions_Level_LevelId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."Level" DROP CONSTRAINT "PK_Level";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."ExamBoard" DROP CONSTRAINT "PK_ExamBoard";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."Level" RENAME TO "Levels";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."ExamBoard" RENAME TO "ExamBoards";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."Levels" ADD CONSTRAINT "PK_Levels" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."ExamBoards" ADD CONSTRAINT "PK_ExamBoards" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    CREATE INDEX "IX_Levels_Id" ON public."Levels" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    CREATE INDEX "IX_ExamBoards_Id" ON public."ExamBoards" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."Questions" ADD CONSTRAINT "FK_Questions_ExamBoards_ExamBoardId" FOREIGN KEY ("ExamBoardId") REFERENCES public."ExamBoards" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    ALTER TABLE public."Questions" ADD CONSTRAINT "FK_Questions_Levels_LevelId" FOREIGN KEY ("LevelId") REFERENCES public."Levels" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422205010_AddedFilterOptionsToModelBuilder') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230422205010_AddedFilterOptionsToModelBuilder', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422214836_ChangedQuestionsAndAssignmentsToHaveAManyToManyWithoutTrackingOnQuestion') THEN
    ALTER TABLE public."Questions" DROP CONSTRAINT "FK_Questions_Assignments_AssignmentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422214836_ChangedQuestionsAndAssignmentsToHaveAManyToManyWithoutTrackingOnQuestion') THEN
    DROP INDEX public."IX_Questions_AssignmentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422214836_ChangedQuestionsAndAssignmentsToHaveAManyToManyWithoutTrackingOnQuestion') THEN
    ALTER TABLE public."Questions" DROP COLUMN "AssignmentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422214836_ChangedQuestionsAndAssignmentsToHaveAManyToManyWithoutTrackingOnQuestion') THEN
    CREATE TABLE public."AssignmentQuestion" (
        "AssignmentId" uuid NOT NULL,
        "QuestionsId" uuid NOT NULL,
        CONSTRAINT "PK_AssignmentQuestion" PRIMARY KEY ("AssignmentId", "QuestionsId"),
        CONSTRAINT "FK_AssignmentQuestion_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_AssignmentQuestion_Questions_QuestionsId" FOREIGN KEY ("QuestionsId") REFERENCES public."Questions" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422214836_ChangedQuestionsAndAssignmentsToHaveAManyToManyWithoutTrackingOnQuestion') THEN
    CREATE INDEX "IX_AssignmentQuestion_QuestionsId" ON public."AssignmentQuestion" ("QuestionsId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230422214836_ChangedQuestionsAndAssignmentsToHaveAManyToManyWithoutTrackingOnQuestion') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230422214836_ChangedQuestionsAndAssignmentsToHaveAManyToManyWithoutTrackingOnQuestion', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423011429_AddedNamePropertyToClassAndToAssignment') THEN
    ALTER TABLE public."Classes" ADD "Name" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423011429_AddedNamePropertyToClassAndToAssignment') THEN
    ALTER TABLE public."Assignments" ADD "Name" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423011429_AddedNamePropertyToClassAndToAssignment') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230423011429_AddedNamePropertyToClassAndToAssignment', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423014243_ChangedAssignmentsFromHaveAOneToOneWithStudentsToEachClassHavingManyAssignmentsAndEachAssignmentHavingOneClass') THEN
    ALTER TABLE public."Assignments" DROP CONSTRAINT "FK_Assignments_Students_StudentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423014243_ChangedAssignmentsFromHaveAOneToOneWithStudentsToEachClassHavingManyAssignmentsAndEachAssignmentHavingOneClass') THEN
    DROP INDEX public."IX_Assignments_StudentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423014243_ChangedAssignmentsFromHaveAOneToOneWithStudentsToEachClassHavingManyAssignmentsAndEachAssignmentHavingOneClass') THEN
    ALTER TABLE public."Assignments" DROP COLUMN "StudentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423014243_ChangedAssignmentsFromHaveAOneToOneWithStudentsToEachClassHavingManyAssignmentsAndEachAssignmentHavingOneClass') THEN
    ALTER TABLE public."Assignments" ADD "ClassId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423014243_ChangedAssignmentsFromHaveAOneToOneWithStudentsToEachClassHavingManyAssignmentsAndEachAssignmentHavingOneClass') THEN
    CREATE INDEX "IX_Assignments_ClassId" ON public."Assignments" ("ClassId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423014243_ChangedAssignmentsFromHaveAOneToOneWithStudentsToEachClassHavingManyAssignmentsAndEachAssignmentHavingOneClass') THEN
    ALTER TABLE public."Assignments" ADD CONSTRAINT "FK_Assignments_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES public."Classes" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230423014243_ChangedAssignmentsFromHaveAOneToOneWithStudentsToEachClassHavingManyAssignmentsAndEachAssignmentHavingOneClass') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230423014243_ChangedAssignmentsFromHaveAOneToOneWithStudentsToEachClassHavingManyAssignmentsAndEachAssignmentHavingOneClass', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425095956_AddedFileNamePropertyToQuestion') THEN
    ALTER TABLE public."Questions" ADD "FileName" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425095956_AddedFileNamePropertyToQuestion') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230425095956_AddedFileNamePropertyToQuestion', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425102743_ChangedDueAndSetInAssignmentsFromDateTimeToDateOnly') THEN
    ALTER TABLE public."Assignments" ALTER COLUMN "Set" TYPE date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425102743_ChangedDueAndSetInAssignmentsFromDateTimeToDateOnly') THEN
    ALTER TABLE public."Assignments" ALTER COLUMN "Due" TYPE date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425102743_ChangedDueAndSetInAssignmentsFromDateTimeToDateOnly') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230425102743_ChangedDueAndSetInAssignmentsFromDateTimeToDateOnly', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425113609_AddedMarksToQuestionsAndAssignments') THEN
    ALTER TABLE public."Questions" ADD "Marks" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425113609_AddedMarksToQuestionsAndAssignments') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230425113609_AddedMarksToQuestionsAndAssignments', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425192340_StopedStudentOnPaperResultFromBeingNullable') THEN
    ALTER TABLE public."PaperResults" DROP CONSTRAINT "FK_PaperResults_Students_StudentId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425192340_StopedStudentOnPaperResultFromBeingNullable') THEN
    UPDATE public."PaperResults" SET "StudentId" = '00000000-0000-0000-0000-000000000000' WHERE "StudentId" IS NULL;
    ALTER TABLE public."PaperResults" ALTER COLUMN "StudentId" SET NOT NULL;
    ALTER TABLE public."PaperResults" ALTER COLUMN "StudentId" SET DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425192340_StopedStudentOnPaperResultFromBeingNullable') THEN
    ALTER TABLE public."PaperResults" ADD CONSTRAINT "FK_PaperResults_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES public."Students" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230425192340_StopedStudentOnPaperResultFromBeingNullable') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230425192340_StopedStudentOnPaperResultFromBeingNullable', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230504203948_ChangedQuestionsToContainAPaperResultPerEachStudentForEachAssignment') THEN
    ALTER TABLE public."PaperResults" ADD "QuestionId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230504203948_ChangedQuestionsToContainAPaperResultPerEachStudentForEachAssignment') THEN
    CREATE INDEX "IX_PaperResults_QuestionId" ON public."PaperResults" ("QuestionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230504203948_ChangedQuestionsToContainAPaperResultPerEachStudentForEachAssignment') THEN
    ALTER TABLE public."PaperResults" ADD CONSTRAINT "FK_PaperResults_Questions_QuestionId" FOREIGN KEY ("QuestionId") REFERENCES public."Questions" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230504203948_ChangedQuestionsToContainAPaperResultPerEachStudentForEachAssignment') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230504203948_ChangedQuestionsToContainAPaperResultPerEachStudentForEachAssignment', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230513224200_DisplayMigration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230513224200_DisplayMigration', '7.0.5');
    END IF;
END $EF$;
COMMIT;

