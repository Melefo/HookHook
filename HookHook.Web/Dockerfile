FROM node:lts-alpine as build
WORKDIR /app

COPY HookHook.Web/package*.json /app/
RUN npm install

COPY HookHook.Web/ /app
RUN npm run build


FROM nginx as production
EXPOSE 80

RUN mkdir /app

COPY --from=build /app/dist /app
COPY HookHook.Web/nginx.conf /etc/nginx/templates/default.conf.template
