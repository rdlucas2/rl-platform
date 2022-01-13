#TODO: build both client and server and run them both in same container
FROM node:14 AS Build

WORKDIR /app
COPY . .
RUN npm ci

WORKDIR /app/webclient
RUN npm ci
RUN npm run build

FROM node:14-alpine AS Deploy
WORKDIR /app
COPY --from=Build /app .
EXPOSE 3000
CMD ["npm", "start"]
