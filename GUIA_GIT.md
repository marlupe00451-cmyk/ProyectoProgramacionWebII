# 🚀 Guía Rápida de Comandos Git

Esta guía contiene los pasos y comandos necesarios para actualizar tu proyecto en GitHub.

---

## 📌 Caso 1: Modificar un detalle o actualización normal

Úsalo cuando hagas cambios en el código (corrección de bugs, estilos, nuevas funciones menores) y quieras subirlos a GitHub sin cambiar de versión principal.

1. **Revisar qué archivos cambiaste:**
   ```powershell
   git status
   ```

2. **Preparar todos los archivos modificados:**
   ```powershell
   git add .
   ```
   *(Si solo quieres agregar un archivo específico: `git add ruta/al/archivo`)*

3. **Guardar los cambios con un mensaje descriptivo:**
   ```powershell
   git commit -m "Descripción de lo que cambiaste o agregaste"
   ```

4. **Subir los cambios a GitHub:**
   ```powershell
   git push
   ```

---

## 🏷️ Caso 2: Crear y subir una nueva versión con Tag

Úsalo cuando termines una etapa importante (por ejemplo: `v1.1.0`, `v1.2.0`, `v2.0.0`) y quieras marcarla como una versión oficial distinguible.

1. **Preparar y guardar los cambios:**
   ```powershell
   git add .
   git commit -m "Finalizada funcionalidad X para la versión 1.1.0"
   ```

2. **Crear la etiqueta (tag) anotada:**
   ```powershell
   git tag -a v1.1.0 -m "Versión 1.1.0 - Resumen de novedades"
   ```

3. **Subir los cambios y el nuevo tag a GitHub:**
   ```powershell
   # Sube el código a la rama main
   git push

   # Sube el nuevo tag (o usa: git push origin --tags)
   git push origin v1.1.0
   ```

---

## 🔍 Caso 3: Comandos útiles de consulta

- **Ver el estado actual del repositorio:**
  ```powershell
  git status
  ```
- **Ver el historial de commits recientes:**
  ```powershell
  git log --oneline -n 10
  ```
- **Ver todos los tags existentes:**
  ```powershell
  git tag -n
  ```
- **Traer cambios desde GitHub (si editaste algo en la web):**
  ```powershell
  git pull
  ```

---

## 💡 Consejos de buenas prácticas para las versiones (SemVer)
El formato estándar es `vX.Y.Z` (ejemplo: `v1.0.0`):
- **Z (Patch - v1.0.1):** Corrección de pequeños bugs o detalles.
- **Y (Minor - v1.1.0):** Nuevas funciones sin romper lo anterior.
- **X (Major - v2.0.0):** Cambios grandes o rediseño completo del sistema.
