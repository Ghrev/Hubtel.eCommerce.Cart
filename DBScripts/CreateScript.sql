create table "User"
(
    "UserId"   serial  not null
        constraint user_pk
            primary key,
    "Msisdn"   varchar,
    "FullName" varchar not null,
    "Password" varchar not null,
    "Username" varchar
);

create table "Items"
(
    "ItemId"    serial  not null
        constraint items_pk
            primary key,
    "ItemName"  varchar not null,
    "UnitPrice" numeric,
    "Quantity"  integer,
    "AddedAt"   timestamp,
    "UpdatedAt" timestamp
);

create table "ShoppingCart"
(
    "ShoppingCartId" serial not null
        constraint shoppingcart_pk
            primary key,
    "ItemId"         integer
        constraint shoppingcart_items_itemid_fk
            references "Items",
    "UserId"         integer
        constraint shoppingcart_user_userid_fk
            references "User",
    "AddedAt"        timestamp,
    "UpdatedAt"      timestamp,
    "Quantity"       integer
);


