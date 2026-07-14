# Utilizar una imagen base que incluya Node.js y npm
FROM node:22 AS builder

# Establecer el directorio de trabajo dentro del contenedor
WORKDIR /app

# Copiar los archivos de dependencias al contenedor
COPY package.json package-lock.json ./

# Instalar las dependencias de forma reproducible
RUN npm ci

# Copiar los archivos de la aplicación al contenedor
COPY . .

# Construir la aplicación
RUN npm run build

# Etapa de producción
FROM nginx:alpine

# Copiar los archivos compilados de Angular desde la etapa anterior
COPY --from=builder /app/dist/darkkitchen/browser/ /usr/share/nginx/html/
COPY nginx.conf /etc/nginx/conf.d/default.conf

# El puerto que escucha NGINX por defecto
EXPOSE 80

# Comando para iniciar NGINX
CMD ["nginx", "-g", "daemon off;"]
