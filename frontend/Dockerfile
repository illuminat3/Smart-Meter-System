FROM node:18-alpine AS builder

WORKDIR /app

ARG VITE_API_BASE_URL=https://localhost:7268
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build

# Prod
FROM node:18-alpine

WORKDIR /app

RUN npm install -g serve

COPY --from=builder /app/dist ./dist

EXPOSE 5173

CMD ["serve", "-s", "dist", "-l", "5173"]