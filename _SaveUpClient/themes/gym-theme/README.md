# gym-theme

Keycloak custom theme scaffold for:
- `login` (also covers register flow)
- `account` (`/realms/<realm>/account`)

## Structure

- `login/theme.properties`
- `login/resources/css/login.css`
- `account/theme.properties`
- `account/resources/css/account.css`

## Mount path in container

Mount `themes` to:

`/opt/keycloak/themes`

Then select in Realm Settings -> Themes:
- Login Theme: `gym-theme`
- Account Theme: `gym-theme`

Also set:
- Realm Settings -> Login -> `User registration`: `ON`

Notes:
- `login` theme covers both sign-in and register pages.
- `account` theme extends `keycloak.v3` for modern account console support.
