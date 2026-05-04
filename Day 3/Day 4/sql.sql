CREATE TABLE products (
id SERIAL PRIMARY KEY,
name TEXT,
details JSONB
);
 
-- sample data
INSERT INTO products (name, details) VALUES
('Laptop', '{"brand": "Apple", "specs": {"ram": "16GB", "storage": "512GB"}}'),
('Phone', '{"brand": "Samsung", "specs": {"ram": "8GB", "storage": "128GB"}}');