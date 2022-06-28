create function "AddItemToShoppingCart"("_itemId" integer, "_userId" integer, "_addedAt" timestamp without time zone, "_updatedAt" timestamp without time zone, _quantity integer) returns integer
    language plpgsql
as
$$
begin
            insert into "ShoppingCart" ("ItemId", "UserId", "AddedAt", "UpdatedAt", "Quantity")
            values ("_itemId", "_userId", "_addedAt", "_updatedAt", _quantity);
            return 1;
        end;
$$;


create function "CreateUser"("_userName" character varying, "_fullName" character varying, _password character varying, _msisdn character varying) returns integer
    language plpgsql
as
$$
BEGIN
            INSERT INTO "User"("Username","FullName","Msisdn", "Password")
            VALUES ("_userName","_fullName", "_msisdn", "_password" );
            RETURN 1;
        end
$$;


create function "GetAllShoppingCarts"()
    returns TABLE("ShoppingCartId" integer, "ItemId" integer, "UserId" integer, "AddedAt" timestamp without time zone, "UpdatedAt" timestamp without time zone, "Quantity" integer, "Fullname" character varying, "PhoneNumber" character varying, "ItemName" character varying, "UnitPrice" numeric)
    language plpgsql
as
$$
begin
            return query
            select S."ShoppingCartId", S."ItemId", S."UserId", S."AddedAt", S."UpdatedAt", S."Quantity", U."FullName", U."Msisdn", I."ItemName",  I."UnitPrice"
            From "ShoppingCart" S, "User" U, "Items" I
            where S."UserId" = U."UserId" and I."ItemId" = S."ItemId" ;
        end;
$$;


create function "GetItemFromCart"("_userId" integer, "_itemId" integer)
    returns TABLE("ItemId" integer, "ItemName" character varying, "AddedAt" timestamp without time zone, "UpdatedAt" timestamp without time zone, "PhoneNumber" character varying, "Owner" character varying, "Quantity" integer)
    language plpgsql
as
$$
begin
            return query
            select S."ItemId", I."ItemName", S."AddedAt", S."UpdatedAt", U."Msisdn", U."FullName", S."Quantity"
            From "Items" I , "User" U, "ShoppingCart" S
            where S."UserId" = U."UserId" and I."ItemId" = S."ItemId" and S."UserId" = "_userId" and S."ItemId" = "_itemId";
        end;
$$;

create function "GetUserById"("_userId" integer)
    returns TABLE("UserName" character varying, "FullName" character varying, "PhoneNumber" character varying, "Password" character varying, "UserId" integer)
    language plpgsql
as
$$
BEGIN
            RETURN QUERY
            SELECT U."Username", U."FullName", U."Msisdn", U."Password", U."UserId"
            FROM "User" U
            WHERE U."UserId" = "_userId";
        end
$$;


create function "GetUserByUsername"(_username character varying)
    returns TABLE("Username" character varying, "FullName" character varying, "PhoneNumber" character varying, "Password" character varying, "UserId" integer)
    language plpgsql
as
$$
BEGIN
            RETURN QUERY
            SELECT U."Username", U."FullName", U."Msisdn", U."Password", U."UserId"
            FROM "User" U
            WHERE U."Username" = "_username";
        end
$$;


create function "ListAllShoppingCartItems"("_userId" integer)
    returns TABLE("ItemId" integer, "ItemName" character varying, "AddedAt" timestamp without time zone, "UpdatedAt" timestamp without time zone, "PhoneNumber" character varying, "Owner" character varying, "Quantity" integer)
    language plpgsql
as
$$
begin
            return query
            select S."ItemId", I."ItemName", S."AddedAt", S."UpdatedAt", U."Msisdn", U."FullName", S."Quantity"
            From "Items" I , "User" U, "ShoppingCart" S
            where S."UserId" = U."UserId" and I."ItemId" = S."ItemId" and S."UserId" = "_userId";
        end;
$$;


create function "ListAllShoppingItems"()
    returns TABLE("ItemId" integer, "ItemName" character varying, "UnitPrice" numeric)
    language plpgsql
as
$$
begin
            return query
            select I."ItemId", I."ItemName", I."UnitPrice"
            from "Items" I;
        end
$$;


create function "RemoveItemFromCart"("_userId" integer, "_itemId" integer) returns integer
    language plpgsql
as
$$
begin
            delete from "ShoppingCart"
            where "ItemId" = "_itemId" and "UserId" = "_userId";
            return 1;
        end;
$$;


create function "UpdateItemQuantity"("_itemId" integer, _quantity integer, "_updatedAt" timestamp without time zone) returns integer
    language plpgsql
as
$$
begin
            UPDATE "ShoppingCart"
            SET "Quantity" = "_quantity",
            "UpdatedAt" = "_updatedAt"
            WHERE "ItemId" = "_itemId";

            return 1;
        end;
$$;
