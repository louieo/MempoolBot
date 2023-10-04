-- Table: public.settings

-- DROP TABLE IF EXISTS public.settings;

CREATE TABLE IF NOT EXISTS public.settings
(
    notify_method character varying(50) COLLATE pg_catalog."default" NOT NULL,
    mempool_api_url character varying(500) COLLATE pg_catalog."default" NOT NULL,
    economy_rate_threshold integer NOT NULL,
    notify_repeat_frequency_seconds integer NOT NULL,
    telegram_bot_token character varying(500) COLLATE pg_catalog."default",
    smtp_server character varying(500) COLLATE pg_catalog."default",
    smtp_user character varying(100) COLLATE pg_catalog."default",
    smtp_pass character varying(100) COLLATE pg_catalog."default",
    from_email character varying(100) COLLATE pg_catalog."default",
    to_email character varying(100) COLLATE pg_catalog."default"
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.settings
    OWNER to postgres;