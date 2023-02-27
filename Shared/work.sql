CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE public."Students" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Password" bytea NOT NULL,
    CONSTRAINT "PK_Students" PRIMARY KEY ("Id")
);

CREATE TABLE public."Teachers" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Password" bytea NOT NULL,
    CONSTRAINT "PK_Teachers" PRIMARY KEY ("Id")
);

CREATE TABLE public."Assignments" (
    "Id" uuid NOT NULL,
    "Set" timestamp with time zone NOT NULL,
    "Due" timestamp with time zone NOT NULL,
    "StudentId" uuid NULL,
    CONSTRAINT "PK_Assignments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Assignments_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES public."Students" ("Id")
);

CREATE TABLE public."Classes" (
    "Id" uuid NOT NULL,
    "TeacherId" uuid NOT NULL,
    CONSTRAINT "PK_Classes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Classes_Teachers_TeacherId" FOREIGN KEY ("TeacherId") REFERENCES public."Teachers" ("Id") ON DELETE CASCADE
);

CREATE TABLE public."PaperResults" (
    "Id" uuid NOT NULL,
    "Score" integer NOT NULL,
    "AssignmentId" uuid NOT NULL,
    "StudentId" uuid NULL,
    CONSTRAINT "PK_PaperResults" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PaperResults_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PaperResults_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES public."Students" ("Id")
);

CREATE TABLE public."Questions" (
    "Id" uuid NOT NULL,
    "Data" bytea NOT NULL,
    "ReadData" text NOT NULL,
    "AssignmentId" uuid NOT NULL,
    CONSTRAINT "PK_Questions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Questions_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments" ("Id") ON DELETE CASCADE
);

CREATE TABLE public."ClassStudent" (
    "CurrentClassesId" uuid NOT NULL,
    "StudentsId" uuid NOT NULL,
    CONSTRAINT "PK_ClassStudent" PRIMARY KEY ("CurrentClassesId", "StudentsId"),
    CONSTRAINT "FK_ClassStudent_Classes_CurrentClassesId" FOREIGN KEY ("CurrentClassesId") REFERENCES public."Classes" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ClassStudent_Students_StudentsId" FOREIGN KEY ("StudentsId") REFERENCES public."Students" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Assignments_Id" ON public."Assignments" ("Id");

CREATE INDEX "IX_Assignments_StudentId" ON public."Assignments" ("StudentId");

CREATE INDEX "IX_Classes_Id" ON public."Classes" ("Id");

CREATE INDEX "IX_Classes_TeacherId" ON public."Classes" ("TeacherId");

CREATE INDEX "IX_ClassStudent_StudentsId" ON public."ClassStudent" ("StudentsId");

CREATE INDEX "IX_PaperResults_AssignmentId" ON public."PaperResults" ("AssignmentId");

CREATE INDEX "IX_PaperResults_Id" ON public."PaperResults" ("Id");

CREATE INDEX "IX_PaperResults_StudentId" ON public."PaperResults" ("StudentId");

CREATE INDEX "IX_Questions_AssignmentId" ON public."Questions" ("AssignmentId");

CREATE INDEX "IX_Questions_Id" ON public."Questions" ("Id");

CREATE INDEX "IX_Students_Id" ON public."Students" ("Id");

CREATE INDEX "IX_Teachers_Id" ON public."Teachers" ("Id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230216011619_One', '7.0.2');

COMMIT;

