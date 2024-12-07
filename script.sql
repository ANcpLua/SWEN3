﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Documents" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    "FilePath" text NOT NULL,
    "DateUploaded" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Documents" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241119202839_Initial', '8.0.10');

COMMIT;

