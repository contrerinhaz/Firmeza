# Configuración de SMTP con Gmail

## Pasos para configurar Gmail SMTP

### 1. Habilitar Verificación en 2 Pasos
1. Ve a tu cuenta de Google: https://myaccount.google.com/
2. En el menú lateral, selecciona **Seguridad**
3. Busca **Verificación en dos pasos** y actívala
4. Sigue los pasos para configurar la autenticación en dos factores

### 2. Generar una Contraseña de Aplicación
1. Una vez habilitada la verificación en 2 pasos, ve a: https://myaccount.google.com/apppasswords
2. En "Seleccionar app", elige **Correo**
3. En "Seleccionar dispositivo", elige **Otro (nombre personalizado)**
4. Escribe un nombre como "Firmeza API"
5. Haz clic en **Generar**
6. Google te mostrará una contraseña de 16 caracteres
7. **Copia esta contraseña** (solo se muestra una vez)

### 3. Configurar appsettings.json
Edita el archivo `Firmeza.Api/appsettings.json`:

```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "tu-email@gmail.com",
  "Password": "xxxx xxxx xxxx xxxx",  // La contraseña de aplicación de 16 caracteres
  "FromName": "Firmeza - Tienda Online"
}
```

**Importante:**
- Usa tu email real de Gmail en `Username`
- Usa la contraseña de aplicación (no tu contraseña normal) en `Password`
- Puedes quitar los espacios de la contraseña si lo deseas

### 4. Para Producción (appsettings.Production.json)
**NUNCA** commits las credenciales reales al repositorio. Usa variables de entorno o Azure Key Vault:

```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "${SMTP_USERNAME}",
  "Password": "${SMTP_PASSWORD}",
  "FromName": "Firmeza - Tienda Online"
}
```

O configúralas como variables de entorno:
```bash
export SMTP_USERNAME=tu-email@gmail.com
export SMTP_PASSWORD=xxxx-xxxx-xxxx-xxxx
```

### 5. Alternativas a Gmail

Si prefieres usar otro proveedor SMTP:

#### SendGrid
```json
"Smtp": {
  "Host": "smtp.sendgrid.net",
  "Port": 587,
  "Username": "apikey",
  "Password": "TU_API_KEY_DE_SENDGRID",
  "FromName": "Firmeza"
}
```

#### Outlook/Office365
```json
"Smtp": {
  "Host": "smtp.office365.com",
  "Port": 587,
  "Username": "tu-email@outlook.com",
  "Password": "tu-contraseña",
  "FromName": "Firmeza"
}
```

#### SMTP Corporativo
```json
"Smtp": {
  "Host": "smtp.tuempresa.com",
  "Port": 587,
  "Username": "tu-usuario",
  "Password": "tu-contraseña",
  "FromName": "Firmeza"
}
```

## Verificación

Una vez configurado, la API enviará correos automáticamente cuando:
- Un usuario realiza una compra (comprobante de compra)
- Se registra un nuevo usuario (opcional)

## Solución de Problemas

### Error: "Authentication failed"
- Verifica que estés usando la contraseña de aplicación, no tu contraseña normal
- Asegúrate de que la verificación en 2 pasos esté activada

### Error: "SMTP connection timeout"
- Verifica que el puerto sea 587
- Verifica tu conexión a Internet
- Algunos firewalls corporativos bloquean el puerto 587

### Los correos llegan a SPAM
- Configura SPF, DKIM y DMARC en tu dominio
- Usa un email corporativo en lugar de Gmail para producción
- Considera usar servicios como SendGrid o Amazon SES

## Seguridad

✅ **Buenas prácticas:**
- Usa contraseñas de aplicación en lugar de contraseñas normales
- No hagas commit de credenciales al repositorio
- Usa variables de entorno en producción
- Limita los permisos de la cuenta de email
- Monitorea el uso de la cuenta para detectar anomalías

❌ **No hagas esto:**
- No uses tu email personal principal
- No deshabilites la verificación en 2 pasos
- No compartas las credenciales SMTP
- No hagas commit del appsettings.json con credenciales reales
