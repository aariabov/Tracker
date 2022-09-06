FROM node:current-alpine as build

WORKDIR /app
ARG NODE_ENV
ENV NODE_ENV $NODE_ENV
COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

FROM nginx
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

# для разработки в docker контейнере
#FROM node:current-alpine
#
#WORKDIR /app
#COPY package*.json ./
#RUN npm install
#
#COPY . .
#CMD ["npm", "run", "start"]