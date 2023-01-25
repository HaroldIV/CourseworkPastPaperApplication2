﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE public."Classes" (
    "Id" uuid NOT NULL,
    CONSTRAINT "PK_Classes" PRIMARY KEY ("Id")
);

CREATE TABLE public."Teachers" (
    "Password" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_Teachers" PRIMARY KEY ("Password")
);

CREATE TABLE public."Students" (
    "Password" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    "ClassId" uuid NOT NULL,
    CONSTRAINT "PK_Students" PRIMARY KEY ("Password"),
    CONSTRAINT "FK_Students_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES public."Classes" ("Id") ON DELETE CASCADE
);

CREATE TABLE public."ClassTeacher" (
    "ClassesId" uuid NOT NULL,
    "TeachersPassword" bigint NOT NULL,
    CONSTRAINT "PK_ClassTeacher" PRIMARY KEY ("ClassesId", "TeachersPassword"),
    CONSTRAINT "FK_ClassTeacher_Classes_ClassesId" FOREIGN KEY ("ClassesId") REFERENCES public."Classes" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ClassTeacher_Teachers_TeachersPassword" FOREIGN KEY ("TeachersPassword") REFERENCES public."Teachers" ("Password") ON DELETE CASCADE
);

CREATE TABLE public."Assignments" (
    "Id" uuid NOT NULL,
    "Set" timestamp with time zone NOT NULL,
    "Due" timestamp with time zone NOT NULL,
    "StudentPassword" bigint NULL,
    CONSTRAINT "PK_Assignments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Assignments_Students_StudentPassword" FOREIGN KEY ("StudentPassword") REFERENCES public."Students" ("Password")
);

CREATE TABLE public."PaperResults" (
    "Id" uuid NOT NULL,
    "Score" integer NOT NULL,
    "AssignmentId" uuid NOT NULL,
    "StudentPassword" bigint NULL,
    CONSTRAINT "PK_PaperResults" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PaperResults_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PaperResults_Students_StudentPassword" FOREIGN KEY ("StudentPassword") REFERENCES public."Students" ("Password")
);

CREATE TABLE public."Questions" (
    "Id" uuid NOT NULL,
    "Data" bytea NOT NULL,
    "ReadData" text NOT NULL,
    "AssignmentId" uuid NULL,
    CONSTRAINT "PK_Questions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Questions_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments" ("Id")
);

CREATE INDEX "IX_Assignments_StudentPassword" ON public."Assignments" ("StudentPassword");

CREATE INDEX "IX_ClassTeacher_TeachersPassword" ON public."ClassTeacher" ("TeachersPassword");

CREATE INDEX "IX_PaperResults_AssignmentId" ON public."PaperResults" ("AssignmentId");

CREATE INDEX "IX_PaperResults_StudentPassword" ON public."PaperResults" ("StudentPassword");

CREATE INDEX "IX_Questions_AssignmentId" ON public."Questions" ("AssignmentId");

CREATE INDEX "IX_Students_ClassId" ON public."Students" ("ClassId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230124102025_test', '7.0.2');

COMMIT;

