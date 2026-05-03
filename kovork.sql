--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5
-- Dumped by pg_dump version 17.5

-- Started on 2026-05-03 20:04:06

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

--
-- TOC entry 10 (class 2615 OID 51150)
-- Name: kovorking; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA kovorking;


ALTER SCHEMA kovorking OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 338 (class 1259 OID 51220)
-- Name: additional_services; Type: TABLE; Schema: kovorking; Owner: postgres
--

CREATE TABLE kovorking.additional_services (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    cost numeric(10,2) NOT NULL,
    description text,
    CONSTRAINT additional_services_cost_check CHECK ((cost >= (0)::numeric))
);


ALTER TABLE kovorking.additional_services OWNER TO postgres;

--
-- TOC entry 337 (class 1259 OID 51219)
-- Name: additional_services_id_seq; Type: SEQUENCE; Schema: kovorking; Owner: postgres
--

CREATE SEQUENCE kovorking.additional_services_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE kovorking.additional_services_id_seq OWNER TO postgres;

--
-- TOC entry 5126 (class 0 OID 0)
-- Dependencies: 337
-- Name: additional_services_id_seq; Type: SEQUENCE OWNED BY; Schema: kovorking; Owner: postgres
--

ALTER SEQUENCE kovorking.additional_services_id_seq OWNED BY kovorking.additional_services.id;


--
-- TOC entry 341 (class 1259 OID 51259)
-- Name: booking_services; Type: TABLE; Schema: kovorking; Owner: postgres
--

CREATE TABLE kovorking.booking_services (
    booking_id integer NOT NULL,
    service_id integer NOT NULL,
    quantity integer DEFAULT 1,
    CONSTRAINT booking_services_quantity_check CHECK ((quantity > 0))
);


ALTER TABLE kovorking.booking_services OWNER TO postgres;

--
-- TOC entry 340 (class 1259 OID 51240)
-- Name: bookings; Type: TABLE; Schema: kovorking; Owner: postgres
--

CREATE TABLE kovorking.bookings (
    id integer NOT NULL,
    user_id integer NOT NULL,
    room_id integer NOT NULL,
    start_time timestamp without time zone NOT NULL,
    duration_hours numeric(5,2) NOT NULL,
    status character varying(20) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT bookings_duration_hours_check CHECK ((duration_hours > (0)::numeric)),
    CONSTRAINT bookings_status_check CHECK (((status)::text = ANY ((ARRAY['Новое'::character varying, 'Подтверждено'::character varying, 'Отменено'::character varying, 'Завершено'::character varying])::text[])))
);


ALTER TABLE kovorking.bookings OWNER TO postgres;

--
-- TOC entry 339 (class 1259 OID 51239)
-- Name: bookings_id_seq; Type: SEQUENCE; Schema: kovorking; Owner: postgres
--

CREATE SEQUENCE kovorking.bookings_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE kovorking.bookings_id_seq OWNER TO postgres;

--
-- TOC entry 5127 (class 0 OID 0)
-- Dependencies: 339
-- Name: bookings_id_seq; Type: SEQUENCE OWNED BY; Schema: kovorking; Owner: postgres
--

ALTER SEQUENCE kovorking.bookings_id_seq OWNED BY kovorking.bookings.id;


--
-- TOC entry 335 (class 1259 OID 51196)
-- Name: equipment; Type: TABLE; Schema: kovorking; Owner: postgres
--

CREATE TABLE kovorking.equipment (
    id integer NOT NULL,
    name character varying(100) NOT NULL
);


ALTER TABLE kovorking.equipment OWNER TO postgres;

--
-- TOC entry 334 (class 1259 OID 51195)
-- Name: equipment_id_seq; Type: SEQUENCE; Schema: kovorking; Owner: postgres
--

CREATE SEQUENCE kovorking.equipment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE kovorking.equipment_id_seq OWNER TO postgres;

--
-- TOC entry 5128 (class 0 OID 0)
-- Dependencies: 334
-- Name: equipment_id_seq; Type: SEQUENCE OWNED BY; Schema: kovorking; Owner: postgres
--

ALTER SEQUENCE kovorking.equipment_id_seq OWNED BY kovorking.equipment.id;


--
-- TOC entry 329 (class 1259 OID 51152)
-- Name: roles; Type: TABLE; Schema: kovorking; Owner: postgres
--

CREATE TABLE kovorking.roles (
    id integer NOT NULL,
    name character varying(30) NOT NULL
);


ALTER TABLE kovorking.roles OWNER TO postgres;

--
-- TOC entry 328 (class 1259 OID 51151)
-- Name: roles_id_seq; Type: SEQUENCE; Schema: kovorking; Owner: postgres
--

CREATE SEQUENCE kovorking.roles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE kovorking.roles_id_seq OWNER TO postgres;

--
-- TOC entry 5129 (class 0 OID 0)
-- Dependencies: 328
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: kovorking; Owner: postgres
--

ALTER SEQUENCE kovorking.roles_id_seq OWNED BY kovorking.roles.id;


--
-- TOC entry 336 (class 1259 OID 51204)
-- Name: room_equipment; Type: TABLE; Schema: kovorking; Owner: postgres
--

CREATE TABLE kovorking.room_equipment (
    room_id integer NOT NULL,
    equipment_id integer NOT NULL
);


ALTER TABLE kovorking.room_equipment OWNER TO postgres;

--
-- TOC entry 333 (class 1259 OID 51178)
-- Name: rooms; Type: TABLE; Schema: kovorking; Owner: postgres
--

CREATE TABLE kovorking.rooms (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    type character varying(50) NOT NULL,
    capacity integer NOT NULL,
    hourly_rate numeric(10,2) NOT NULL,
    min_hours integer DEFAULT 1 NOT NULL,
    discount_percent numeric(5,2) DEFAULT 0,
    description text,
    rating numeric(3,2),
    photo_url character varying(255),
    CONSTRAINT rooms_capacity_check CHECK ((capacity > 0)),
    CONSTRAINT rooms_discount_percent_check CHECK (((discount_percent >= (0)::numeric) AND (discount_percent <= (100)::numeric))),
    CONSTRAINT rooms_hourly_rate_check CHECK ((hourly_rate >= (0)::numeric)),
    CONSTRAINT rooms_min_hours_check CHECK ((min_hours > 0)),
    CONSTRAINT rooms_rating_check CHECK (((rating >= (0)::numeric) AND (rating <= (5)::numeric)))
);


ALTER TABLE kovorking.rooms OWNER TO postgres;

--
-- TOC entry 332 (class 1259 OID 51177)
-- Name: rooms_id_seq; Type: SEQUENCE; Schema: kovorking; Owner: postgres
--

CREATE SEQUENCE kovorking.rooms_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE kovorking.rooms_id_seq OWNER TO postgres;

--
-- TOC entry 5130 (class 0 OID 0)
-- Dependencies: 332
-- Name: rooms_id_seq; Type: SEQUENCE OWNED BY; Schema: kovorking; Owner: postgres
--

ALTER SEQUENCE kovorking.rooms_id_seq OWNED BY kovorking.rooms.id;


--
-- TOC entry 331 (class 1259 OID 51161)
-- Name: users; Type: TABLE; Schema: kovorking; Owner: postgres
--

CREATE TABLE kovorking.users (
    id integer NOT NULL,
    role_id integer NOT NULL,
    full_name character varying(100) NOT NULL,
    email character varying(100) NOT NULL,
    phone character varying(20),
    password_hash character varying(255) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE kovorking.users OWNER TO postgres;

--
-- TOC entry 330 (class 1259 OID 51160)
-- Name: users_id_seq; Type: SEQUENCE; Schema: kovorking; Owner: postgres
--

CREATE SEQUENCE kovorking.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE kovorking.users_id_seq OWNER TO postgres;

--
-- TOC entry 5131 (class 0 OID 0)
-- Dependencies: 330
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: kovorking; Owner: postgres
--

ALTER SEQUENCE kovorking.users_id_seq OWNED BY kovorking.users.id;


--
-- TOC entry 4914 (class 2604 OID 51223)
-- Name: additional_services id; Type: DEFAULT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.additional_services ALTER COLUMN id SET DEFAULT nextval('kovorking.additional_services_id_seq'::regclass);


--
-- TOC entry 4915 (class 2604 OID 51243)
-- Name: bookings id; Type: DEFAULT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.bookings ALTER COLUMN id SET DEFAULT nextval('kovorking.bookings_id_seq'::regclass);


--
-- TOC entry 4913 (class 2604 OID 51199)
-- Name: equipment id; Type: DEFAULT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.equipment ALTER COLUMN id SET DEFAULT nextval('kovorking.equipment_id_seq'::regclass);


--
-- TOC entry 4907 (class 2604 OID 51155)
-- Name: roles id; Type: DEFAULT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.roles ALTER COLUMN id SET DEFAULT nextval('kovorking.roles_id_seq'::regclass);


--
-- TOC entry 4910 (class 2604 OID 51181)
-- Name: rooms id; Type: DEFAULT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.rooms ALTER COLUMN id SET DEFAULT nextval('kovorking.rooms_id_seq'::regclass);


--
-- TOC entry 4908 (class 2604 OID 51164)
-- Name: users id; Type: DEFAULT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.users ALTER COLUMN id SET DEFAULT nextval('kovorking.users_id_seq'::regclass);


--
-- TOC entry 5117 (class 0 OID 51220)
-- Dependencies: 338
-- Data for Name: additional_services; Type: TABLE DATA; Schema: kovorking; Owner: postgres
--

COPY kovorking.additional_services (id, name, cost, description) FROM stdin;
1	Аренда ноутбука	500.00	Ноутбук с установленным ПО
2	Кофе-брейк	300.00	Кофе, чай, печенье на человека
3	Флипчарт	400.00	Маркерная доска с комплектом маркеров
4	Проектор дополнительный	600.00	Переносной проектор
\.


--
-- TOC entry 5120 (class 0 OID 51259)
-- Dependencies: 341
-- Data for Name: booking_services; Type: TABLE DATA; Schema: kovorking; Owner: postgres
--

COPY kovorking.booking_services (booking_id, service_id, quantity) FROM stdin;
1	3	1
2	2	1
2	1	1
3	1	1
\.


--
-- TOC entry 5119 (class 0 OID 51240)
-- Dependencies: 340
-- Data for Name: bookings; Type: TABLE DATA; Schema: kovorking; Owner: postgres
--

COPY kovorking.bookings (id, user_id, room_id, start_time, duration_hours, status, created_at) FROM stdin;
1	5	1	2025-04-15 10:00:00	2.00	Подтверждено	2025-04-01 00:00:00
4	6	3	2025-04-18 09:00:00	8.00	Отменено	2025-04-03 00:00:00
6	5	5	2026-04-29 13:15:00	8.00	Отменено	2026-04-29 10:15:51.88582
7	5	5	2026-04-03 10:42:00	15.00	Отменено	2026-04-29 10:42:39.904094
5	5	3	2026-04-02 02:32:00	5.00	Отменено	2026-04-29 00:32:31.571394
2	6	2	2025-04-16 14:00:00	3.00	Подтверждено	2025-04-02 00:00:00
3	5	4	2025-04-20 11:00:00	4.00	Отменено	2025-04-05 00:00:00
\.


--
-- TOC entry 5114 (class 0 OID 51196)
-- Dependencies: 335
-- Data for Name: equipment; Type: TABLE DATA; Schema: kovorking; Owner: postgres
--

COPY kovorking.equipment (id, name) FROM stdin;
1	Wi-Fi
2	проектор
3	маркерная доска
4	кондиционер
5	кулер
6	розетки
7	флипчарт
8	акустика
9	микрофоны
\.


--
-- TOC entry 5108 (class 0 OID 51152)
-- Dependencies: 329
-- Data for Name: roles; Type: TABLE DATA; Schema: kovorking; Owner: postgres
--

COPY kovorking.roles (id, name) FROM stdin;
1	Администратор
2	Оператор
3	Клиент
\.


--
-- TOC entry 5115 (class 0 OID 51204)
-- Dependencies: 336
-- Data for Name: room_equipment; Type: TABLE DATA; Schema: kovorking; Owner: postgres
--

COPY kovorking.room_equipment (room_id, equipment_id) FROM stdin;
1	1
1	2
1	3
2	1
2	2
2	4
3	1
3	5
3	6
4	1
4	2
4	4
4	7
5	1
5	2
5	8
5	9
6	1
6	4
\.


--
-- TOC entry 5112 (class 0 OID 51178)
-- Dependencies: 333
-- Data for Name: rooms; Type: TABLE DATA; Schema: kovorking; Owner: postgres
--

COPY kovorking.rooms (id, name, type, capacity, hourly_rate, min_hours, discount_percent, description, rating, photo_url) FROM stdin;
1	Лофт №1	Переговорная	6	1200.00	2	0.00	Уютная переговорная с панорамным окном	4.20	C:\\Users\\sekibanki\\RiderProjects\\kovorking\\kovorking\\PhotoRoom\\loft1.jpg
2	Зеленая комната	Переговорная	8	1500.00	2	10.00	Оформлена живыми растениями	4.70	C:\\Users\\sekibanki\\RiderProjects\\kovorking\\kovorking\\PhotoRoom\\green-room.jpg
3	Open Space	Рабочее место	20	350.00	1	0.00	Общая зона с комфортными креслами	4.00	C:\\Users\\sekibanki\\RiderProjects\\kovorking\\kovorking\\PhotoRoom\\open-space-room.jpg
4	Бизнес-зал	Переговорная	12	2000.00	2	15.00	Премиум-класс, кожаные кресла	4.80	C:\\Users\\sekibanki\\RiderProjects\\kovorking\\kovorking\\PhotoRoom\\busines_room.jpg
5	Медиа-студия	Лекторий	30	3500.00	3	0.00	Для презентаций и вебинаров	4.50	C:\\Users\\sekibanki\\RiderProjects\\kovorking\\kovorking\\PhotoRoom\\media-room.jpg
6	Кабинет №5	Офис	4	800.00	1	5.00	Изолированный кабинет для переговоров	4.10	C:\\Users\\sekibanki\\RiderProjects\\kovorking\\kovorking\\PhotoRoom\\cab5.jpg
\.


--
-- TOC entry 5110 (class 0 OID 51161)
-- Dependencies: 331
-- Data for Name: users; Type: TABLE DATA; Schema: kovorking; Owner: postgres
--

COPY kovorking.users (id, role_id, full_name, email, phone, password_hash, created_at) FROM stdin;
1	1	Воронов Андрей Михайлович	a.voronov@cowork.ru	79161234567	admin2024	2026-04-22 13:13:40.19264
2	1	Белова Екатерина Дмитриевна	e.belova@cowork.ru	79269876543	admin2024	2026-04-22 13:13:40.19264
3	2	Новиков Павел Сергеевич	p.novikov@cowork.ru	79031234567	oper123	2026-04-22 13:13:40.19264
4	2	Морозова Ирина Алексеевна	i.morozova@cowork.ru	79151112233	oper123	2026-04-22 13:13:40.19264
5	3	Тимофеев Артем Денисович	artem.t@mail.ru	79264567890	client321	2026-04-22 13:13:40.19264
6	3	Крылова Светлана Игоревна	svetlana.k@yandex.ru	79097654321	client321	2026-04-22 13:13:40.19264
\.


--
-- TOC entry 5132 (class 0 OID 0)
-- Dependencies: 337
-- Name: additional_services_id_seq; Type: SEQUENCE SET; Schema: kovorking; Owner: postgres
--

SELECT pg_catalog.setval('kovorking.additional_services_id_seq', 4, true);


--
-- TOC entry 5133 (class 0 OID 0)
-- Dependencies: 339
-- Name: bookings_id_seq; Type: SEQUENCE SET; Schema: kovorking; Owner: postgres
--

SELECT pg_catalog.setval('kovorking.bookings_id_seq', 7, true);


--
-- TOC entry 5134 (class 0 OID 0)
-- Dependencies: 334
-- Name: equipment_id_seq; Type: SEQUENCE SET; Schema: kovorking; Owner: postgres
--

SELECT pg_catalog.setval('kovorking.equipment_id_seq', 9, true);


--
-- TOC entry 5135 (class 0 OID 0)
-- Dependencies: 328
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: kovorking; Owner: postgres
--

SELECT pg_catalog.setval('kovorking.roles_id_seq', 3, true);


--
-- TOC entry 5136 (class 0 OID 0)
-- Dependencies: 332
-- Name: rooms_id_seq; Type: SEQUENCE SET; Schema: kovorking; Owner: postgres
--

SELECT pg_catalog.setval('kovorking.rooms_id_seq', 6, true);


--
-- TOC entry 5137 (class 0 OID 0)
-- Dependencies: 330
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: kovorking; Owner: postgres
--

SELECT pg_catalog.setval('kovorking.users_id_seq', 6, true);


--
-- TOC entry 4948 (class 2606 OID 51230)
-- Name: additional_services additional_services_name_key; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.additional_services
    ADD CONSTRAINT additional_services_name_key UNIQUE (name);


--
-- TOC entry 4950 (class 2606 OID 51228)
-- Name: additional_services additional_services_pkey; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.additional_services
    ADD CONSTRAINT additional_services_pkey PRIMARY KEY (id);


--
-- TOC entry 4954 (class 2606 OID 51265)
-- Name: booking_services booking_services_pkey; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.booking_services
    ADD CONSTRAINT booking_services_pkey PRIMARY KEY (booking_id, service_id);


--
-- TOC entry 4952 (class 2606 OID 51248)
-- Name: bookings bookings_pkey; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.bookings
    ADD CONSTRAINT bookings_pkey PRIMARY KEY (id);


--
-- TOC entry 4942 (class 2606 OID 51203)
-- Name: equipment equipment_name_key; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.equipment
    ADD CONSTRAINT equipment_name_key UNIQUE (name);


--
-- TOC entry 4944 (class 2606 OID 51201)
-- Name: equipment equipment_pkey; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.equipment
    ADD CONSTRAINT equipment_pkey PRIMARY KEY (id);


--
-- TOC entry 4928 (class 2606 OID 51159)
-- Name: roles roles_name_key; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.roles
    ADD CONSTRAINT roles_name_key UNIQUE (name);


--
-- TOC entry 4930 (class 2606 OID 51157)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- TOC entry 4946 (class 2606 OID 51208)
-- Name: room_equipment room_equipment_pkey; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.room_equipment
    ADD CONSTRAINT room_equipment_pkey PRIMARY KEY (room_id, equipment_id);


--
-- TOC entry 4938 (class 2606 OID 51194)
-- Name: rooms rooms_name_key; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.rooms
    ADD CONSTRAINT rooms_name_key UNIQUE (name);


--
-- TOC entry 4940 (class 2606 OID 51192)
-- Name: rooms rooms_pkey; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.rooms
    ADD CONSTRAINT rooms_pkey PRIMARY KEY (id);


--
-- TOC entry 4932 (class 2606 OID 51169)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 4934 (class 2606 OID 51171)
-- Name: users users_phone_key; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.users
    ADD CONSTRAINT users_phone_key UNIQUE (phone);


--
-- TOC entry 4936 (class 2606 OID 51167)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 4960 (class 2606 OID 51266)
-- Name: booking_services booking_services_booking_id_fkey; Type: FK CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.booking_services
    ADD CONSTRAINT booking_services_booking_id_fkey FOREIGN KEY (booking_id) REFERENCES kovorking.bookings(id) ON DELETE CASCADE;


--
-- TOC entry 4961 (class 2606 OID 51271)
-- Name: booking_services booking_services_service_id_fkey; Type: FK CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.booking_services
    ADD CONSTRAINT booking_services_service_id_fkey FOREIGN KEY (service_id) REFERENCES kovorking.additional_services(id) ON DELETE CASCADE;


--
-- TOC entry 4958 (class 2606 OID 51254)
-- Name: bookings bookings_room_id_fkey; Type: FK CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.bookings
    ADD CONSTRAINT bookings_room_id_fkey FOREIGN KEY (room_id) REFERENCES kovorking.rooms(id) ON DELETE RESTRICT;


--
-- TOC entry 4959 (class 2606 OID 51249)
-- Name: bookings bookings_user_id_fkey; Type: FK CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.bookings
    ADD CONSTRAINT bookings_user_id_fkey FOREIGN KEY (user_id) REFERENCES kovorking.users(id) ON DELETE CASCADE;


--
-- TOC entry 4956 (class 2606 OID 51214)
-- Name: room_equipment room_equipment_equipment_id_fkey; Type: FK CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.room_equipment
    ADD CONSTRAINT room_equipment_equipment_id_fkey FOREIGN KEY (equipment_id) REFERENCES kovorking.equipment(id) ON DELETE CASCADE;


--
-- TOC entry 4957 (class 2606 OID 51209)
-- Name: room_equipment room_equipment_room_id_fkey; Type: FK CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.room_equipment
    ADD CONSTRAINT room_equipment_room_id_fkey FOREIGN KEY (room_id) REFERENCES kovorking.rooms(id) ON DELETE CASCADE;


--
-- TOC entry 4955 (class 2606 OID 51172)
-- Name: users users_role_id_fkey; Type: FK CONSTRAINT; Schema: kovorking; Owner: postgres
--

ALTER TABLE ONLY kovorking.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES kovorking.roles(id) ON DELETE RESTRICT;


-- Completed on 2026-05-03 20:04:06

--
-- PostgreSQL database dump complete
--

