CREATE TABLE "user"
(
    id         UUID PRIMARY KEY,
    name       VARCHAR(100) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ
);

CREATE TABLE "Habit"
(
    id          UUID PRIMARY KEY,
    name        VARCHAR(250) NOT NULL,
    user_id     UUID NOT NULL,
    created_at  TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES "user" (id)    
);


CREATE TABLE "Days_off"
(
    id          UUID PRIMARY KEY,
    habit_id    UUID NOT NULL ,
    days        VARCHAR(250) NOT NULL,
    FOREIGN KEY (habit_id) REFERENCES "Habit" (id) ON DELETE CASCADE ON UPDATE CASCADE
);



CREATE TABLE "Badge"
(
    id          UUID PRIMARY KEY,
    name        VARCHAR(250) NOT NULL,
    description VARCHAR(250),
    user_id     UUID NOT NULL,
    created_at  TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES "user" (id)
);



CREATE TABLE "Logs"
(
    id          UUID PRIMARY KEY,
    habit_id    UUID NOT NULL,
    log_date    TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (habit_id) REFERENCES "Habit" (id) ON DELETE CASCADE ON UPDATE CASCADE
);

DROP TABLE "Logs"

CREATE TABLE "Streak"
(
    id              UUID PRIMARY KEY,
    habit_id        UUID NOT NULL,
    current_streak  INT NOT NULL,
    longest_streak  INT NOT NULL,
    created_at      TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (habit_id) REFERENCES "Habit" (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE "Streak_snapshot"
(
    id                      UUID PRIMARY KEY,
    habit_id                UUID NOT NULL,
    current_streak          INT NOT NULL,
    longest_streak          INT NOT NULL,
    last_streak_id          UUID NOT NULL,
    last_streak_created_at  TIMESTAMPTZ NOT NULL,
    created_at              TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (habit_id) REFERENCES "Habit" (id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (last_streak_id) REFERENCES "Streak" (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE "StreakCheck"(
    id              UUID PRIMARY KEY,
    log_libur       INT,
    log_epic        INT,
    user_id         UUID NOT NULL,
    habit_id        UUID NOT NULL,
    FOREIGN KEY (user_id) REFERENCES "user" (id),
    FOREIGN KEY (habit_id) REFERENCES "Habit" (id) ON DELETE CASCADE ON UPDATE CASCADE
);


DROP TABLE "Streak"

SELECT * FROM "Streak"

SELECT logs FROM "Logs" l JOIN "Habit" h ON l.habit_id = h.id 