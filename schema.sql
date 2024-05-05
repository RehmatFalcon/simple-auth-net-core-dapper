create database simple_auth;

-- Use database and run commands there

CREATE TABLE auth_user (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    user_status VARCHAR(50) DEFAULT 'Active',
    user_type VARCHAR(50) DEFAULT 'NormalUser',
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);