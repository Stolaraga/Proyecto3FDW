
"""
Crear datos demo para el reporte "vacunacion-proxima-semana" y consultar el reporte.
Uso:
  python crear_demo_reporte.py --base http://localhost:5194
  (también funciona con HTTPS: --base https://localhost:7043 --insecure)
"""
from __future__ import annotations
import argparse
import datetime as dt
import json
import sys
from typing import Any, Dict

import requests

def build_session(insecure: bool) -> requests.Session:
    s = requests.Session()
    if insecure:
        s.verify = False  # ignora certificado dev
        try:
            import urllib3
            urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
        except Exception:
            pass
    s.headers.update({"Content-Type": "application/json"})
    return s

def post_json(s: requests.Session, base: str, path: str, body: Dict[str, Any]) -> Dict[str, Any]:
    url = f"{base.rstrip('/')}{path}"
    try:
        r = s.post(url, data=json.dumps(body), timeout=15)
        r.raise_for_status()
        return r.json()
    except requests.exceptions.RequestException as e:
        print(f"[POST {url}] ERROR: {e}", file=sys.stderr)
        if hasattr(e, "response") and getattr(e, "response", None) is not None:
            print(f"Status: {e.response.status_code}\nBody: {e.response.text}", file=sys.stderr)
        sys.exit(1)

def get_json(s: requests.Session, base: str, path: str):
    url = f"{base.rstrip('/')}{path}"
    try:
        r = s.get(url, timeout=15)
        r.raise_for_status()
        return r.json()
    except requests.exceptions.RequestException as e:
        print(f"[GET {url}] ERROR: {e}", file=sys.stderr)
        if hasattr(e, "response") and getattr(e, "response", None) is not None:
            print(f"Status: {e.response.status_code}\nBody: {e.response.text}", file=sys.stderr)
        sys.exit(1)

def ultima_vacuna_para_proxima_en_dias(dias: int) -> str:
    """Devuelve fecha (YYYY-MM-DD) de la *última* vacuna para que la próxima sea en N días."""
    hoy = dt.date.today()
    ultima = hoy - dt.timedelta(days=(365 - dias))
    return ultima.strftime("%Y-%m-%d")

def main():
    parser = argparse.ArgumentParser(description="Crea datos demo y consulta el reporte de vacunación próxima semana.")
    parser.add_argument("--base", default="http://localhost:5194", help="Base URL de la API (http o https). Ej: http://localhost:5194")
    parser.add_argument("--insecure", action="store_true", help="Ignorar certificado TLS (útil con https de desarrollo).")
    args = parser.parse_args()

    s = build_session(args.insecure)

    print(f"Usando API base: {args.base}")
    # Prueba rápida de conexión (Swagger)
    try:
        _ = get_json(s, args.base, "/swagger/v1/swagger.json")
        print("OK: swagger.json responde")
    except SystemExit:
        # Algunos templates hostean Swagger sólo en Development con UI; si falla, seguimos intentándolo con endpoints
        print("Aviso: no se pudo leer swagger.json; continuo con los endpoints...", file=sys.stderr)

    # 1) Cliente
    cliente = post_json(s, args.base, "/api/clientes", {
        "cedula": "900100200",
        "nombre": "Carla",
        "apellidos": "Lopez",
        "email": "carla@example.com",
        "telefono": "70000011",
        "canalPreferido": 1,
        "direccion": "San Jose"
    })
    cliente_id = cliente["id"]
    print(f"Cliente creado: {cliente_id}")

    # 2) Empleado
    empleado = post_json(s, args.base, "/api/empleados", {
        "nombre": "Maria",
        "apellidos": "Vargas",
        "email": "mvargas@example.com",
        "telefono": "70000012",
        "rol": 1
    })
    empleado_id = empleado["id"]
    print(f"Empleado creado: {empleado_id}")

    # 3) Mascotas (3)
    m1 = post_json(s, args.base, "/api/mascotas", {"clienteId": cliente_id, "nombre": "Firu",  "especie": 1, "sexo": 1})
    m2 = post_json(s, args.base, "/api/mascotas", {"clienteId": cliente_id, "nombre": "Mishi", "especie": 2, "sexo": 2})
    m3 = post_json(s, args.base, "/api/mascotas", {"clienteId": cliente_id, "nombre": "Paco",  "especie": 3, "sexo": 1})
    m1_id, m2_id, m3_id = m1["id"], m2["id"], m3["id"]
    print("Mascotas:", m1_id, m2_id, m3_id)

    # 4) Atenciones de Vacunación Anual con próximas en 3/5/7 días
    fechas = [ultima_vacuna_para_proxima_en_dias(d) for d in (3, 5, 7)]
    cuerpos = [
        {"mascotaId": m1_id, "clienteId": cliente_id, "empleadoId": empleado_id, "tipo": 1, "fecha": fechas[0], "notas": "Próxima en 3 días"},
        {"mascotaId": m2_id, "clienteId": cliente_id, "empleadoId": empleado_id, "tipo": 1, "fecha": fechas[1], "notas": "Próxima en 5 días"},
        {"mascotaId": m3_id, "clienteId": cliente_id, "empleadoId": empleado_id, "tipo": 1, "fecha": fechas[2], "notas": "Próxima en 7 días"},
    ]
    for body in cuerpos:
        post_json(s, args.base, "/api/atenciones", body)

    # 5) Consultar reporte
    reporte = get_json(s, args.base, "/api/reportes/vacunacion-proxima-semana")
    print("\n== REPORTE ==")
    print(json.dumps(reporte, indent=2, ensure_ascii=False))

if __name__ == "__main__":
    main()
