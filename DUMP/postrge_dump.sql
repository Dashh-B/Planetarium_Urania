--
-- PostgreSQL database dump
--

\restrict NcFnhyOwMfvdROZXgNLmJrHSzk1SqiPTmMhQ9uezqekoHpgjTzCfYkKgzkDAvnd

-- Dumped from database version 18.2
-- Dumped by pg_dump version 18.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: app_users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.app_users (
    id integer NOT NULL,
    username character varying(50) NOT NULL,
    password_hash character varying(64) NOT NULL,
    role character varying(20) NOT NULL,
    is_active boolean DEFAULT true,
    CONSTRAINT app_users_role_check CHECK (((role)::text = ANY ((ARRAY['admin'::character varying, 'manager'::character varying, 'viewer'::character varying])::text[])))
);


ALTER TABLE public.app_users OWNER TO postgres;

--
-- Name: app_users_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.app_users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.app_users_id_seq OWNER TO postgres;

--
-- Name: app_users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.app_users_id_seq OWNED BY public.app_users.id;


--
-- Name: astronomical_objects; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.astronomical_objects (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    name_latin character varying(255),
    object_type character varying(50) NOT NULL,
    right_ascension character varying(20),
    declination character varying(20),
    magnitude numeric(4,2),
    constellation character varying(100),
    visibility_months character varying(100),
    observation_conditions character varying(100),
    description text,
    CONSTRAINT astronomical_objects_object_type_check CHECK (((object_type)::text = ANY ((ARRAY['планета'::character varying, 'звезда'::character varying, 'туманность'::character varying, 'галактика'::character varying, 'созвездие'::character varying, 'комета'::character varying, 'астероид'::character varying])::text[]))),
    CONSTRAINT astronomical_objects_observation_conditions_check CHECK (((observation_conditions)::text = ANY ((ARRAY['невооруженным глазом'::character varying, 'бинокль'::character varying, 'телескоп'::character varying])::text[])))
);


ALTER TABLE public.astronomical_objects OWNER TO postgres;

--
-- Name: TABLE astronomical_objects; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.astronomical_objects IS 'Справочник астрономических объектов';


--
-- Name: astronomical_objects_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.astronomical_objects_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.astronomical_objects_id_seq OWNER TO postgres;

--
-- Name: astronomical_objects_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.astronomical_objects_id_seq OWNED BY public.astronomical_objects.id;


--
-- Name: employees; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employees (
    id integer NOT NULL,
    full_name character varying(255) NOT NULL,
    "position" character varying(100) NOT NULL,
    specialization character varying(255),
    phone character varying(20) NOT NULL,
    email character varying(100),
    login character varying(50) NOT NULL,
    password_hash character varying(255) NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    CONSTRAINT employees_position_check CHECK ((("position")::text = ANY ((ARRAY['астроном'::character varying, 'лектор'::character varying, 'экскурсовод'::character varying, 'кассир'::character varying, 'администратор'::character varying])::text[])))
);


ALTER TABLE public.employees OWNER TO postgres;

--
-- Name: TABLE employees; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.employees IS 'Сотрудники планетария';


--
-- Name: employees_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.employees_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employees_id_seq OWNER TO postgres;

--
-- Name: employees_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employees_id_seq OWNED BY public.employees.id;


--
-- Name: observations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.observations (
    id integer NOT NULL,
    observation_date date NOT NULL,
    observation_time time without time zone NOT NULL,
    telescope_id integer NOT NULL,
    object_id integer NOT NULL,
    astronomer_id integer NOT NULL,
    weather_conditions character varying(255),
    observation_type character varying(50) NOT NULL,
    participants_count integer,
    notes text,
    CONSTRAINT observations_observation_type_check CHECK (((observation_type)::text = ANY ((ARRAY['публичное'::character varying, 'научное'::character varying, 'образовательное'::character varying, 'фотосессия'::character varying])::text[]))),
    CONSTRAINT observations_participants_count_check CHECK ((participants_count >= 0))
);


ALTER TABLE public.observations OWNER TO postgres;

--
-- Name: TABLE observations; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.observations IS 'Журнал наблюдений';


--
-- Name: observations_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.observations_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.observations_id_seq OWNER TO postgres;

--
-- Name: observations_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.observations_id_seq OWNED BY public.observations.id;


--
-- Name: program_objects; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.program_objects (
    id integer NOT NULL,
    program_id integer NOT NULL,
    object_id integer NOT NULL,
    order_in_program integer
);


ALTER TABLE public.program_objects OWNER TO postgres;

--
-- Name: TABLE program_objects; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.program_objects IS 'Связь программ с демонстрируемыми объектами';


--
-- Name: program_objects_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.program_objects_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.program_objects_id_seq OWNER TO postgres;

--
-- Name: program_objects_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.program_objects_id_seq OWNED BY public.program_objects.id;


--
-- Name: programs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.programs (
    id integer NOT NULL,
    title character varying(255) NOT NULL,
    theme character varying(100) NOT NULL,
    duration integer NOT NULL,
    age_rating character varying(10) NOT NULL,
    description text,
    price numeric(10,2) NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    CONSTRAINT programs_age_rating_check CHECK (((age_rating)::text = ANY ((ARRAY['0+'::character varying, '6+'::character varying, '12+'::character varying, '16+'::character varying])::text[]))),
    CONSTRAINT programs_duration_check CHECK ((duration > 0)),
    CONSTRAINT programs_price_check CHECK ((price >= (0)::numeric)),
    CONSTRAINT programs_theme_check CHECK (((theme)::text = ANY ((ARRAY['Солнечная система'::character varying, 'дальний космос'::character varying, 'история астрономии'::character varying, 'детская программа'::character varying, 'специальная программа'::character varying])::text[])))
);


ALTER TABLE public.programs OWNER TO postgres;

--
-- Name: TABLE programs; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.programs IS 'Программы показов планетария';


--
-- Name: programs_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.programs_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.programs_id_seq OWNER TO postgres;

--
-- Name: programs_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.programs_id_seq OWNED BY public.programs.id;


--
-- Name: sessions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.sessions (
    id integer NOT NULL,
    program_id integer NOT NULL,
    session_date date NOT NULL,
    session_time time without time zone NOT NULL,
    hall character varying(50) NOT NULL,
    lecturer_id integer,
    capacity integer NOT NULL,
    tickets_sold integer DEFAULT 0 NOT NULL,
    status character varying(20) DEFAULT 'запланирован'::character varying NOT NULL,
    CONSTRAINT check_tickets_capacity CHECK ((tickets_sold <= capacity)),
    CONSTRAINT sessions_capacity_check CHECK ((capacity > 0)),
    CONSTRAINT sessions_hall_check CHECK (((hall)::text = ANY ((ARRAY['основной купол'::character varying, 'малый зал'::character varying, 'лекторий'::character varying])::text[]))),
    CONSTRAINT sessions_status_check CHECK (((status)::text = ANY ((ARRAY['запланирован'::character varying, 'завершен'::character varying, 'отменен'::character varying])::text[]))),
    CONSTRAINT sessions_tickets_sold_check CHECK ((tickets_sold >= 0))
);


ALTER TABLE public.sessions OWNER TO postgres;

--
-- Name: TABLE sessions; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.sessions IS 'Сеансы показов';


--
-- Name: sessions_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.sessions_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.sessions_id_seq OWNER TO postgres;

--
-- Name: sessions_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.sessions_id_seq OWNED BY public.sessions.id;


--
-- Name: telescopes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.telescopes (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    telescope_type character varying(50) NOT NULL,
    aperture integer,
    focal_length integer,
    location character varying(100) NOT NULL,
    status character varying(20) DEFAULT 'доступен'::character varying NOT NULL,
    is_public boolean DEFAULT false NOT NULL,
    CONSTRAINT telescopes_aperture_check CHECK ((aperture > 0)),
    CONSTRAINT telescopes_focal_length_check CHECK ((focal_length > 0)),
    CONSTRAINT telescopes_status_check CHECK (((status)::text = ANY ((ARRAY['доступен'::character varying, 'на обслуживании'::character varying, 'в резерве'::character varying])::text[]))),
    CONSTRAINT telescopes_telescope_type_check CHECK (((telescope_type)::text = ANY ((ARRAY['рефрактор'::character varying, 'рефлектор'::character varying, 'радиотелескоп'::character varying, 'катадиоптрический'::character varying])::text[])))
);


ALTER TABLE public.telescopes OWNER TO postgres;

--
-- Name: TABLE telescopes; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.telescopes IS 'Телескопы и оборудование';


--
-- Name: telescopes_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.telescopes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.telescopes_id_seq OWNER TO postgres;

--
-- Name: telescopes_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.telescopes_id_seq OWNED BY public.telescopes.id;


--
-- Name: tickets; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tickets (
    id integer NOT NULL,
    ticket_number character varying(50) NOT NULL,
    session_id integer NOT NULL,
    visitor_id integer NOT NULL,
    purchase_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    price numeric(10,2) NOT NULL,
    payment_method character varying(20) NOT NULL,
    status character varying(20) DEFAULT 'оплачен'::character varying NOT NULL,
    CONSTRAINT tickets_payment_method_check CHECK (((payment_method)::text = ANY ((ARRAY['наличные'::character varying, 'карта'::character varying, 'онлайн'::character varying])::text[]))),
    CONSTRAINT tickets_price_check CHECK ((price >= (0)::numeric)),
    CONSTRAINT tickets_status_check CHECK (((status)::text = ANY ((ARRAY['оплачен'::character varying, 'использован'::character varying, 'возвращен'::character varying])::text[])))
);


ALTER TABLE public.tickets OWNER TO postgres;

--
-- Name: TABLE tickets; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.tickets IS 'Проданные билеты';


--
-- Name: tickets_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tickets_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tickets_id_seq OWNER TO postgres;

--
-- Name: tickets_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tickets_id_seq OWNED BY public.tickets.id;


--
-- Name: visitors; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.visitors (
    id integer NOT NULL,
    full_name character varying(255) NOT NULL,
    phone character varying(20) NOT NULL,
    email character varying(100),
    birth_date date,
    category character varying(20) NOT NULL,
    registration_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT visitors_category_check CHECK (((category)::text = ANY ((ARRAY['взрослый'::character varying, 'детский'::character varying, 'студент'::character varying, 'пенсионер'::character varying])::text[])))
);


ALTER TABLE public.visitors OWNER TO postgres;

--
-- Name: TABLE visitors; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.visitors IS 'Посетители планетария';


--
-- Name: visitors_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.visitors_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.visitors_id_seq OWNER TO postgres;

--
-- Name: visitors_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.visitors_id_seq OWNED BY public.visitors.id;


--
-- Name: app_users id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.app_users ALTER COLUMN id SET DEFAULT nextval('public.app_users_id_seq'::regclass);


--
-- Name: astronomical_objects id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.astronomical_objects ALTER COLUMN id SET DEFAULT nextval('public.astronomical_objects_id_seq'::regclass);


--
-- Name: employees id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees ALTER COLUMN id SET DEFAULT nextval('public.employees_id_seq'::regclass);


--
-- Name: observations id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observations ALTER COLUMN id SET DEFAULT nextval('public.observations_id_seq'::regclass);


--
-- Name: program_objects id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.program_objects ALTER COLUMN id SET DEFAULT nextval('public.program_objects_id_seq'::regclass);


--
-- Name: programs id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.programs ALTER COLUMN id SET DEFAULT nextval('public.programs_id_seq'::regclass);


--
-- Name: sessions id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.sessions ALTER COLUMN id SET DEFAULT nextval('public.sessions_id_seq'::regclass);


--
-- Name: telescopes id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.telescopes ALTER COLUMN id SET DEFAULT nextval('public.telescopes_id_seq'::regclass);


--
-- Name: tickets id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets ALTER COLUMN id SET DEFAULT nextval('public.tickets_id_seq'::regclass);


--
-- Name: visitors id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitors ALTER COLUMN id SET DEFAULT nextval('public.visitors_id_seq'::regclass);


--
-- Data for Name: app_users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.app_users (id, username, password_hash, role, is_active) FROM stdin;
1	admin	6d4525c2a21f9be1cca9e41f3aa402e0765ee5fcc3e7fea34a169b1730ae386e	admin	t
2	manager	a69ae21824e5590ce83123122c2c9f4c5855c696c289627f8f4a55741d218c37	manager	t
3	viewer	5b157b4671d659ead967bd90457bb19f5f9376282e33faea14f62af13d5a9414	viewer	t
\.


--
-- Data for Name: astronomical_objects; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.astronomical_objects (id, name, name_latin, object_type, right_ascension, declination, magnitude, constellation, visibility_months, observation_conditions, description) FROM stdin;
1	Луна	Luna	планета	-	-	-12.74	различные	круглый год	невооруженным глазом	Естественный спутник Земли. Единственное небесное тело, на котором побывал человек.
2	Марс	Mars	планета	02h 19m	+13° 00'	-2.94	различные	круглый год	невооруженным глазом	Четвертая планета от Солнца. Красная планета с полярными шапками и признаками древних водных потоков.
3	Юпитер	Jupiter	планета	05h 55m	+22° 30'	-2.70	различные	круглый год	невооруженным глазом	Крупнейшая планета Солнечной системы. Газовый гигант с Большим Красным Пятном.
4	Сатурн	Saturn	планета	14h 15m	-11° 10'	0.46	различные	круглый год	телескоп	Шестая планета от Солнца, известная своими эффектными кольцами.
5	Туманность Ориона	Orion Nebula (M42)	туманность	05h 35m	-05° 27'	4.00	Орион	ноябрь-март	бинокль	Ближайшая к Земле область звездообразования. Видна невооруженным глазом как туманное пятно.
6	Туманность Андромеды	Andromeda Galaxy (M31)	галактика	00h 42m	+41° 16'	3.44	Андромеда	сентябрь-февраль	бинокль	Ближайшая к нам крупная галактика. Находится на расстоянии 2,5 млн световых лет.
7	Плеяды	Pleiades (M45)	звезда	03h 47m	+24° 07'	1.60	Телец	октябрь-апрель	невооруженным глазом	Рассеянное звездное скопление. Включает семь ярких звезд, видимых невооруженным глазом.
8	Полярная звезда	Polaris	звезда	02h 31m	+89° 15'	1.98	Малая Медведица	круглый год	невооруженным глазом	Ближайшая к северному полюсу мира яркая звезда. Используется для навигации.
9	Сириус	Sirius	звезда	06h 45m	-16° 43'	-1.46	Большой Пес	декабрь-март	невооруженным глазом	Ярчайшая звезда ночного неба. Двойная звездная система.
10	Крабовидная туманность	Crab Nebula (M1)	туманность	05h 34m	+22° 01'	8.40	Телец	ноябрь-март	телескоп	Остаток сверхновой, взорвавшейся в 1054 году. Наблюдалась китайскими астрономами.
11	Большое Магелланово Облако	Large Magellanic Cloud	галактика	05h 23m	-69° 45'	0.90	Золотая Рыба	круглый год (ЮП)	невооруженным глазом	Карликовая галактика-спутник Млечного Пути. Видна только в южном полушарии.
12	Венера	Venus	планета	12h 47m	-02° 54'	-4.60	различные	круглый год	невооруженным глазом	Вторая планета от Солнца. Ярчайший объект на небе после Солнца и Луны.
13	Меркурий	Mercury	планета	13h 41m	-12° 02'	-0.70	различные	март-апрель, сентябрь-октябрь	невооруженным глазом	Ближайшая к Солнцу планета. Трудна для наблюдения из-за близости к Солнцу.
14	Большая Медведица	Ursa Major	созвездие	-	-	\N	-	круглый год	невооруженным глазом	Одно из наиболее узнаваемых созвездий северного полушария. Содержит астеризм Большой Ковш.
15	Орион	Orion	созвездие	-	-	\N	-	ноябрь-март	невооруженным глазом	Экваториальное созвездие. Один из самых красивых участков звездного неба.
\.


--
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employees (id, full_name, "position", specialization, phone, email, login, password_hash, is_active) FROM stdin;
1	Иванов Дмитрий Сергеевич	администратор	\N	+7-495-123-45-67	ivanov@planetarium.ru	admin	$2a$10$N9qo8uLOickgx2ZMRZoMye	t
2	Петрова Анна Владимировна	астроном	астрофизика	+7-495-234-56-78	petrova@planetarium.ru	petrova_av	$2a$10$N9qo8uLOickgx2ZMRZoMye	t
3	Сидоров Михаил Александрович	лектор	популяризация науки	+7-495-345-67-89	sidorov@planetarium.ru	sidorov_ma	$2a$10$N9qo8uLOickgx2ZMRZoMye	t
4	Козлова Елена Игоревна	лектор	планетология	+7-495-456-78-90	kozlova@planetarium.ru	kozlova_ei	$2a$10$N9qo8uLOickgx2ZMRZoMye	t
5	Новиков Павел Дмитриевич	астроном	наблюдательная астрономия	+7-495-567-89-01	novikov@planetarium.ru	novikov_pd	$2a$10$N9qo8uLOickgx2ZMRZoMye	t
6	Смирнова Ольга Петровна	экскурсовод	история астрономии	+7-495-678-90-12	smirnova@planetarium.ru	smirnova_op	$2a$10$N9qo8uLOickgx2ZMRZoMye	t
7	Морозов Игорь Викторович	кассир	\N	+7-495-789-01-23	morozov@planetarium.ru	morozov_iv	$2a$10$N9qo8uLOickgx2ZMRZoMye	t
8	Волкова Мария Сергеевна	лектор	популяризация науки	+7-495-890-12-34	volkova@planetarium.ru	volkova_ms	$2a$10$N9qo8uLOickgx2ZMRZoMye	t
\.


--
-- Data for Name: observations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.observations (id, observation_date, observation_time, telescope_id, object_id, astronomer_id, weather_conditions, observation_type, participants_count, notes) FROM stdin;
1	2026-01-15	21:00:00	1	1	2	ясно, небольшая облачность	публичное	15	Наблюдение лунных кратеров. Видны Тихо, Коперник, Море Ясности.
2	2026-01-18	22:30:00	2	4	5	ясно	публичное	12	Отличная видимость колец Сатурна. Заметны деление Кассини и спутник Титан.
3	2026-01-20	20:00:00	1	5	2	ясно	образовательное	25	Групповое наблюдение туманности Ориона со школьниками. Видны четыре звезды трапеции.
4	2026-01-22	23:00:00	3	6	5	отличная прозрачность	научное	3	Фотометрия галактики Андромеды. Получены снимки спиральной структуры.
5	2026-01-25	19:30:00	4	3	2	переменная облачность	публичное	8	Наблюдение Юпитера и его спутников. Видны все четыре галилеевых спутника.
6	2026-01-27	21:00:00	1	7	5	ясно	публичное	18	Наблюдение рассеянного скопления Плеяды. Разрешено более 20 звезд.
7	2026-01-28	22:00:00	2	2	2	хорошая видимость	образовательное	10	Наблюдение Марса. Заметны полярные шапки и темные области.
8	2026-01-29	20:30:00	1	9	5	отличная прозрачность	публичное	14	Наблюдение Сириуса. Объяснение двойных звездных систем.
9	2026-01-30	23:30:00	3	10	2	ясно	фотосессия	2	Астрофотография Крабовидной туманности. Экспозиция 30 минут.
10	2026-01-31	21:30:00	2	12	5	небольшая дымка	публичное	9	Наблюдение Венеры в фазе. Видна серповидная форма планеты.
\.


--
-- Data for Name: program_objects; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.program_objects (id, program_id, object_id, order_in_program) FROM stdin;
1	1	13	1
2	1	12	2
3	1	2	3
4	1	3	4
5	1	4	5
6	2	5	1
7	2	6	2
8	2	10	3
9	2	11	4
10	3	1	1
11	3	14	2
12	3	7	3
13	3	15	4
14	4	8	1
15	4	3	2
16	4	4	3
17	4	1	4
18	5	2	1
19	5	12	2
20	5	9	3
21	5	5	4
22	6	2	1
23	6	3	2
24	6	4	3
25	7	15	1
26	7	14	2
27	7	7	3
\.


--
-- Data for Name: programs; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.programs (id, title, theme, duration, age_rating, description, price, is_active) FROM stdin;
1	Путешествие по Солнечной системе	Солнечная система	45	6+	Увлекательное путешествие от Солнца к внешним границам нашей планетной системы. Изучение всех восьми планет и их особенностей.	500.00	t
2	Тайны дальнего космоса	дальний космос	60	12+	Погружение в глубины Вселенной: галактики, туманности, черные дыры и квазары. Современные космические открытия.	600.00	t
3	Звездное небо для малышей	детская программа	30	0+	Первое знакомство с астрономией для самых маленьких. Истории о созвездиях и планетах в игровой форме.	350.00	t
4	История астрономии: от древности до наших дней	история астрономии	50	12+	Как менялись представления человечества о космосе. От древних обсерваторий до современных телескопов.	550.00	t
5	Живое небо: наблюдаем сегодня	специальная программа	40	6+	Актуальная карта звездного неба на сегодняшний день. Что можно увидеть в телескоп сегодня вечером.	450.00	t
6	Космические миссии XXI века	специальная программа	55	12+	Современные космические программы: Mars Rover, телескоп James Webb, миссии к спутникам Сатурна и Юпитера.	600.00	t
7	Мифы и легенды звездного неба	детская программа	40	6+	Древнегреческие мифы о созвездиях. Истории героев, запечатленных на небосводе.	400.00	t
\.


--
-- Data for Name: sessions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.sessions (id, program_id, session_date, session_time, hall, lecturer_id, capacity, tickets_sold, status) FROM stdin;
1	1	2026-02-01	10:00:00	основной купол	3	80	65	запланирован
2	1	2026-02-01	14:00:00	основной купол	4	80	72	запланирован
3	2	2026-02-01	16:00:00	основной купол	2	80	45	запланирован
4	3	2026-02-02	11:00:00	малый зал	8	40	38	запланирован
5	3	2026-02-02	13:00:00	малый зал	8	40	35	запланирован
6	4	2026-02-02	15:00:00	лекторий	6	50	28	запланирован
7	5	2026-02-03	18:00:00	основной купол	3	80	55	запланирован
8	6	2026-02-03	19:30:00	лекторий	2	50	42	запланирован
9	7	2026-02-04	12:00:00	малый зал	4	40	30	запланирован
10	1	2026-01-28	10:00:00	основной купол	3	80	80	завершен
11	2	2026-01-28	14:00:00	основной купол	2	80	76	завершен
12	3	2026-01-29	11:00:00	малый зал	8	40	40	завершен
13	5	2026-01-30	18:00:00	основной купол	3	80	60	завершен
14	1	2026-02-05	15:00:00	основной купол	4	80	0	отменен
\.


--
-- Data for Name: telescopes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.telescopes (id, name, telescope_type, aperture, focal_length, location, status, is_public) FROM stdin;
1	Celestron NexStar 8SE	катадиоптрический	203	2032	обсерватория	доступен	t
2	Meade LX90	катадиоптрический	305	3048	обсерватория	доступен	t
3	Sky-Watcher Dob 16"	рефлектор	406	1800	обсерватория	доступен	f
4	Takahashi TSA-120	рефрактор	120	900	выездная площадка	доступен	t
5	Orion SkyQuest XT10	рефлектор	254	1200	обсерватория	на обслуживании	t
6	William Optics GT81	рефрактор	81	478	выездная площадка	доступен	f
\.


--
-- Data for Name: tickets; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.tickets (id, ticket_number, session_id, visitor_id, purchase_date, price, payment_method, status) FROM stdin;
1	T-2026-000001	1	1	2026-01-28 09:15:00	500.00	карта	оплачен
2	T-2026-000002	1	2	2026-01-28 09:20:00	500.00	карта	оплачен
3	T-2026-000003	1	3	2026-01-28 10:30:00	400.00	онлайн	оплачен
4	T-2026-000004	2	4	2026-01-28 11:45:00	600.00	наличные	оплачен
5	T-2026-000005	2	5	2026-01-28 12:00:00	300.00	карта	оплачен
6	T-2026-000006	3	6	2026-01-29 08:30:00	300.00	наличные	оплачен
7	T-2026-000007	4	7	2026-01-30 09:00:00	280.00	онлайн	оплачен
8	T-2026-000008	4	8	2026-01-30 09:05:00	350.00	карта	оплачен
9	T-2026-000009	5	9	2026-01-30 10:15:00	350.00	онлайн	оплачен
10	T-2026-000010	10	10	2026-01-27 14:20:00	250.00	наличные	использован
11	T-2026-000011	10	11	2026-01-27 14:25:00	400.00	карта	использован
12	T-2026-000012	11	12	2026-01-27 15:30:00	480.00	онлайн	использован
13	T-2026-000013	11	1	2026-01-27 16:00:00	600.00	карта	использован
14	T-2026-000014	12	2	2026-01-28 10:10:00	280.00	онлайн	использован
15	T-2026-000015	13	3	2026-01-29 17:00:00	360.00	карта	использован
16	T-2026-000016	6	4	2026-01-31 13:00:00	550.00	онлайн	оплачен
17	T-2026-000017	7	5	2026-02-01 10:00:00	225.00	наличные	оплачен
18	T-2026-000018	8	6	2026-02-01 11:30:00	480.00	карта	оплачен
19	T-2026-000019	14	7	2026-02-03 09:00:00	400.00	онлайн	возвращен
20	T-2026-000020	9	8	2026-02-03 15:00:00	400.00	карта	оплачен
\.


--
-- Data for Name: visitors; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.visitors (id, full_name, phone, email, birth_date, category, registration_date) FROM stdin;
1	Александров Алексей Иванович	+7-916-123-45-67	alexandrov@mail.ru	1985-03-15	взрослый	2025-12-10 14:30:00
2	Борисова Светлана Петровна	+7-916-234-56-78	borisova@gmail.com	1990-07-22	взрослый	2025-12-15 10:15:00
3	Васильев Дмитрий Сергеевич	+7-916-345-67-89	vasiliev@yandex.ru	2005-11-08	студент	2026-01-05 16:45:00
4	Григорьева Анна Михайловна	+7-916-456-78-90	grigorieva@mail.ru	1978-04-30	взрослый	2026-01-10 11:20:00
5	Данилов Иван Александрович	+7-916-567-89-01	\N	2015-09-12	детский	2026-01-12 13:00:00
6	Егорова Мария Владимировна	+7-916-678-90-12	egorova@gmail.com	1955-06-18	пенсионер	2026-01-15 09:30:00
7	Жуков Олег Николаевич	+7-916-789-01-23	zhukov@yandex.ru	2003-02-25	студент	2026-01-18 15:10:00
8	Зайцева Елена Игоревна	+7-916-890-12-34	zaitseva@mail.ru	1988-12-03	взрослый	2026-01-20 12:40:00
9	Иванова Ольга Петровна	+7-916-901-23-45	ivanova_op@gmail.com	1992-08-17	взрослый	2026-01-22 14:55:00
10	Ковалев Сергей Викторович	+7-916-012-34-56	kovalev@yandex.ru	2010-05-20	детский	2026-01-25 10:05:00
11	Лебедева Татьяна Андреевна	+7-917-123-45-67	lebedeva@mail.ru	1965-10-14	пенсионер	2026-01-26 16:30:00
12	Морозов Андрей Дмитриевич	+7-917-234-56-78	morozov_ad@gmail.com	2004-01-09	студент	2026-01-27 11:15:00
\.


--
-- Name: app_users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.app_users_id_seq', 3, true);


--
-- Name: astronomical_objects_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.astronomical_objects_id_seq', 30, true);


--
-- Name: employees_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_id_seq', 9, true);


--
-- Name: observations_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.observations_id_seq', 10, true);


--
-- Name: program_objects_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.program_objects_id_seq', 27, true);


--
-- Name: programs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.programs_id_seq', 14, true);


--
-- Name: sessions_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.sessions_id_seq', 14, true);


--
-- Name: telescopes_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.telescopes_id_seq', 6, true);


--
-- Name: tickets_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tickets_id_seq', 20, true);


--
-- Name: visitors_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.visitors_id_seq', 12, true);


--
-- Name: app_users app_users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.app_users
    ADD CONSTRAINT app_users_pkey PRIMARY KEY (id);


--
-- Name: app_users app_users_username_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.app_users
    ADD CONSTRAINT app_users_username_key UNIQUE (username);


--
-- Name: astronomical_objects astronomical_objects_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.astronomical_objects
    ADD CONSTRAINT astronomical_objects_pkey PRIMARY KEY (id);


--
-- Name: employees employees_login_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_login_key UNIQUE (login);


--
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (id);


--
-- Name: observations observations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observations
    ADD CONSTRAINT observations_pkey PRIMARY KEY (id);


--
-- Name: program_objects program_objects_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.program_objects
    ADD CONSTRAINT program_objects_pkey PRIMARY KEY (id);


--
-- Name: programs programs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.programs
    ADD CONSTRAINT programs_pkey PRIMARY KEY (id);


--
-- Name: sessions sessions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.sessions
    ADD CONSTRAINT sessions_pkey PRIMARY KEY (id);


--
-- Name: telescopes telescopes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.telescopes
    ADD CONSTRAINT telescopes_pkey PRIMARY KEY (id);


--
-- Name: tickets tickets_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_pkey PRIMARY KEY (id);


--
-- Name: tickets tickets_ticket_number_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_ticket_number_key UNIQUE (ticket_number);


--
-- Name: program_objects unique_program_object; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.program_objects
    ADD CONSTRAINT unique_program_object UNIQUE (program_id, object_id);


--
-- Name: visitors visitors_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitors
    ADD CONSTRAINT visitors_pkey PRIMARY KEY (id);


--
-- Name: idx_astronomical_objects_type; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_astronomical_objects_type ON public.astronomical_objects USING btree (object_type);


--
-- Name: idx_employees_login; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_employees_login ON public.employees USING btree (login);


--
-- Name: idx_observations_astronomer; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_observations_astronomer ON public.observations USING btree (astronomer_id);


--
-- Name: idx_observations_date; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_observations_date ON public.observations USING btree (observation_date);


--
-- Name: idx_observations_object; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_observations_object ON public.observations USING btree (object_id);


--
-- Name: idx_observations_telescope; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_observations_telescope ON public.observations USING btree (telescope_id);


--
-- Name: idx_program_objects_object; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_program_objects_object ON public.program_objects USING btree (object_id);


--
-- Name: idx_program_objects_program; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_program_objects_program ON public.program_objects USING btree (program_id);


--
-- Name: idx_sessions_date; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_sessions_date ON public.sessions USING btree (session_date);


--
-- Name: idx_sessions_lecturer; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_sessions_lecturer ON public.sessions USING btree (lecturer_id);


--
-- Name: idx_sessions_program; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_sessions_program ON public.sessions USING btree (program_id);


--
-- Name: idx_tickets_purchase_date; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tickets_purchase_date ON public.tickets USING btree (purchase_date);


--
-- Name: idx_tickets_session; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tickets_session ON public.tickets USING btree (session_id);


--
-- Name: idx_tickets_visitor; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tickets_visitor ON public.tickets USING btree (visitor_id);


--
-- Name: idx_visitors_email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_visitors_email ON public.visitors USING btree (email);


--
-- Name: idx_visitors_phone; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_visitors_phone ON public.visitors USING btree (phone);


--
-- Name: observations observations_astronomer_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observations
    ADD CONSTRAINT observations_astronomer_id_fkey FOREIGN KEY (astronomer_id) REFERENCES public.employees(id) ON DELETE RESTRICT;


--
-- Name: observations observations_object_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observations
    ADD CONSTRAINT observations_object_id_fkey FOREIGN KEY (object_id) REFERENCES public.astronomical_objects(id) ON DELETE RESTRICT;


--
-- Name: observations observations_telescope_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observations
    ADD CONSTRAINT observations_telescope_id_fkey FOREIGN KEY (telescope_id) REFERENCES public.telescopes(id) ON DELETE RESTRICT;


--
-- Name: program_objects program_objects_object_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.program_objects
    ADD CONSTRAINT program_objects_object_id_fkey FOREIGN KEY (object_id) REFERENCES public.astronomical_objects(id) ON DELETE CASCADE;


--
-- Name: program_objects program_objects_program_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.program_objects
    ADD CONSTRAINT program_objects_program_id_fkey FOREIGN KEY (program_id) REFERENCES public.programs(id) ON DELETE CASCADE;


--
-- Name: sessions sessions_lecturer_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.sessions
    ADD CONSTRAINT sessions_lecturer_id_fkey FOREIGN KEY (lecturer_id) REFERENCES public.employees(id) ON DELETE SET NULL;


--
-- Name: sessions sessions_program_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.sessions
    ADD CONSTRAINT sessions_program_id_fkey FOREIGN KEY (program_id) REFERENCES public.programs(id) ON DELETE RESTRICT;


--
-- Name: tickets tickets_session_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_session_id_fkey FOREIGN KEY (session_id) REFERENCES public.sessions(id) ON DELETE RESTRICT;


--
-- Name: tickets tickets_visitor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_visitor_id_fkey FOREIGN KEY (visitor_id) REFERENCES public.visitors(id) ON DELETE RESTRICT;


--
-- Name: TABLE app_users; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.app_users TO urania_auth;
GRANT SELECT ON TABLE public.app_users TO urania_viewer;
GRANT SELECT ON TABLE public.app_users TO urania_manager;
GRANT ALL ON TABLE public.app_users TO urania_admin;


--
-- Name: TABLE astronomical_objects; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.astronomical_objects TO urania_viewer;
GRANT SELECT ON TABLE public.astronomical_objects TO urania_manager;
GRANT ALL ON TABLE public.astronomical_objects TO urania_admin;


--
-- Name: TABLE employees; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.employees TO urania_viewer;
GRANT SELECT ON TABLE public.employees TO urania_manager;
GRANT ALL ON TABLE public.employees TO urania_admin;


--
-- Name: TABLE observations; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.observations TO urania_viewer;
GRANT SELECT,INSERT,UPDATE ON TABLE public.observations TO urania_manager;
GRANT ALL ON TABLE public.observations TO urania_admin;


--
-- Name: TABLE program_objects; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.program_objects TO urania_viewer;
GRANT SELECT ON TABLE public.program_objects TO urania_manager;
GRANT ALL ON TABLE public.program_objects TO urania_admin;


--
-- Name: TABLE programs; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.programs TO urania_viewer;
GRANT SELECT,INSERT,UPDATE ON TABLE public.programs TO urania_manager;
GRANT ALL ON TABLE public.programs TO urania_admin;


--
-- Name: TABLE sessions; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.sessions TO urania_viewer;
GRANT SELECT ON TABLE public.sessions TO urania_manager;
GRANT ALL ON TABLE public.sessions TO urania_admin;


--
-- Name: TABLE telescopes; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.telescopes TO urania_viewer;
GRANT SELECT ON TABLE public.telescopes TO urania_manager;
GRANT ALL ON TABLE public.telescopes TO urania_admin;


--
-- Name: TABLE tickets; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.tickets TO urania_viewer;
GRANT SELECT ON TABLE public.tickets TO urania_manager;
GRANT ALL ON TABLE public.tickets TO urania_admin;


--
-- Name: TABLE visitors; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT ON TABLE public.visitors TO urania_viewer;
GRANT SELECT ON TABLE public.visitors TO urania_manager;
GRANT ALL ON TABLE public.visitors TO urania_admin;


--
-- PostgreSQL database dump complete
--

\unrestrict NcFnhyOwMfvdROZXgNLmJrHSzk1SqiPTmMhQ9uezqekoHpgjTzCfYkKgzkDAvnd

