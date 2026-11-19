# Traffic generator

Sends varied traffic to Orders.Api and Users.Api so dashboards have data. Plain Node 20+, no install.

```bash
node load/generate.mjs --rps 10 --duration 120
node load/generate.mjs --help
```

| Flag | Default |
|---|---|
| `--rps` | 5 (average; wobbles ±60% on a 30s sine wave plus noise) |
| `--duration` | 60 seconds (0 = until Ctrl-C) |
| `--base-orders` | `http://localhost:5084` |
| `--base-users` | `http://localhost:5026` |

Mix: 30% list, 35% get-by-id (~25% random GUIDs, so 404/500 paths occur), 25% create, 10% delete. Prints a summary every 5s; Ctrl-C prints totals and exits.

"Slow calls" are whatever the APIs themselves produce under load; the script doesn't inject latency.
