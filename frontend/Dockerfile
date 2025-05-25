FROM node:20 AS builder
WORKDIR /app

# Install deps and build
COPY package*.json ./
RUN npm install
COPY . .
ENV REACT_APP_BASE_URL=http://localhost:5000
RUN npm run build

# Serve with `serve`
RUN npm install -g serve
CMD ["serve", "-s", "build", "-l", "3000"]
