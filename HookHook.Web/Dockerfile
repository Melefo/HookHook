FROM node:lts-alpine
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY ["HookHook.Web/", "/app"]
RUN npm install

CMD ["npm", "run", "dev"]