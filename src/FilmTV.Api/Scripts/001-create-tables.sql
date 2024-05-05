CREATE SEQUENCE IF NOT EXISTS user_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

CREATE TABLE IF NOT EXISTS "user"
(
    id integer NOT NULL DEFAULT nextval('user_id_seq'::regclass),
    auth0_id text NOT NULL,
    CONSTRAINT user_pkey PRIMARY KEY (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_user_auth0_id
    ON "user" USING btree (auth0_id ASC NULLS LAST);

CREATE TABLE IF NOT EXISTS movie
(
    id integer NOT NULL,
    imdb_id text,
    original_title text,
    original_language text,
    release_date date,
    runtime integer,
    etag text,
    CONSTRAINT movie_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS user_movie
(
    movie_id integer NOT NULL,
    user_id integer NOT NULL,
    title text,
    watched_date date,
    rating integer,
    rated timestamp with time zone,
    CONSTRAINT user_move_pkey PRIMARY KEY (user_id, movie_id),
    CONSTRAINT fk_user_movie_movie FOREIGN KEY (movie_id)
        REFERENCES movie (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT fk_user_movie_user FOREIGN KEY (user_id)
        REFERENCES "user" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE TABLE IF NOT EXISTS tv_show
(
    id integer NOT NULL,
    original_title text,
    status text,
    etag text,
    next_update timestamp with time zone,    
    imdb_id text,
    tvdb_id integer,
    CONSTRAINT tv_show_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS user_tv_show
(
    tv_show_id integer NOT NULL,
    user_id integer NOT NULL,
    title text,
    rating integer,
    rated timestamp with time zone,
    CONSTRAINT user_tv_show_pkey PRIMARY KEY (user_id, tv_show_id),
    CONSTRAINT fk_user_tv_show_tv_show FOREIGN KEY (tv_show_id)
        REFERENCES tv_show (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT fk_user_tv_show_user FOREIGN KEY (user_id)
        REFERENCES "user" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE SEQUENCE IF NOT EXISTS episode_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

CREATE TABLE IF NOT EXISTS episode
(
    id integer NOT NULL DEFAULT nextval('episode_id_seq'::regclass),
    tv_show_id integer NOT NULL,
    season_number integer NOT NULL,
    episode_number integer NOT NULL,
    title text,
    air_date date,
    CONSTRAINT episode_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS user_episode
(
    episode_id integer NOT NULL,
    user_id integer NOT NULL,
    watched_date date,    
    CONSTRAINT user_episode_pkey PRIMARY KEY (user_id, episode_id),
    CONSTRAINT fk_user_episode_episode FOREIGN KEY (episode_id)
        REFERENCES episode (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT fk_user_episode_user FOREIGN KEY (user_id)
        REFERENCES "user" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);