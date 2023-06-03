const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:38125';

const PROXY_CONFIG = [
  {
    context: [
      "/listamedicamente",
      "/listapacienti",
      "/listacomenzi",

      "/pacient/adauga",
      "/pacient/modifica",
      "/pacient/sterge",

      "/medicament/loturi",
      "/medicament/loturi/adauga",
      "/medicament/adauga",
      "/medicament/modifica",
      "/medicament/sterge",

      "/comanda/adauga",
      "/comanda/aproba",
      "/comanda/modifica",
      "/comanda/sterge"
   ],
    target: target,
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    }
  }
]

module.exports = PROXY_CONFIG;
