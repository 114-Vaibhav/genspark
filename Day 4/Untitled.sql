3CREATE TABLE products (
id SERIAL PRIMARY KEY,
name TEXT,
details JSONB
);
 
-- sample data
INSERT INTO products (name, details) VALUES
('Laptop', '{"brand": "Apple", "specs": {"ram": "16GB", "storage": "512GB"}}'),
('Phone', '{"brand": "Samsung", "specs": {"ram": "8GB", "storage": "128GB"}}');

-- ('Laptop', '{"brand": "Apple", "specs": {"ram": "16GB", "storage": "512GB"}}'),
INSERT INTO products (name, details) VALUES
('Desktop', '{"brand": "Samsung", "specs": {"ram": "8GB", "storage": "128GB"},"colour":"blue"}');


select * from products;
select details from products;

select details->'brand' from products;

select * from products 
where details->>'brand'='Apple';

select * from products
where details->'specs'->>'ram'='8GB';

select * from products 
where details?'brand';

select * from products 
where details?'colour';
