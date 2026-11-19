#!/usr/bin/env node
// Traffic generator for Orders.Api / Users.Api. No dependencies, Node 20+.
import { parseArgs } from 'node:util';

const HELP = `Usage: node generate.mjs [options]
  --rps <n>            average requests/second (default 5)
  --duration <sec>     run time in seconds, 0 = until Ctrl-C (default 60)
  --base-orders <url>  Orders.Api base URL (default http://localhost:5084)
  --base-users <url>   Users.Api base URL  (default http://localhost:5026)
  -h, --help           show this help
The rate wobbles around --rps (sine wave, 30s period, plus noise).`;

const { values: o } = parseArgs({
  options: {
    rps: { type: 'string', default: '5' },
    duration: { type: 'string', default: '60' },
    'base-orders': { type: 'string', default: 'http://localhost:5084' },
    'base-users': { type: 'string', default: 'http://localhost:5026' },
    help: { type: 'boolean', short: 'h' },
  },
});
if (o.help) { console.log(HELP); process.exit(0); }

const rps = Number(o.rps), duration = Number(o.duration);
if (!(rps > 0) || !(duration >= 0)) { console.error('--rps must be > 0 and --duration >= 0\n\n' + HELP); process.exit(1); }

const rand = (n) => Math.floor(Math.random() * n);
const pick = (a) => a[rand(a.length)];
const uuid = () => crypto.randomUUID();

const targets = [
  { base: o['base-orders'].replace(/\/$/, ''), path: '/api/orders', ids: [],
    body: () => ({ id: uuid(), amount: +(Math.random() * 500).toFixed(2), status: pick(['Pending', 'Paid', 'Shipped', 'Cancelled']), createdAt: new Date().toISOString() }) },
  { base: o['base-users'].replace(/\/$/, ''), path: '/api/users', ids: [],
    body: () => { const n = uuid().slice(0, 8); return { id: uuid(), name: `user-${n}`, email: `${n}@example.com`, createdAt: new Date().toISOString() }; } },
];

const stats = { sent: 0, ok: 0, errors: 0 }; // errors = non-2xx or network failure
const byStatus = {};
const inflight = new Set();

async function fire() {
  const t = pick(targets);
  // Weights: list 30, get-by-id 35, create 25, delete 10.
  const r = rand(100);
  let method = 'GET', url = t.base + t.path, init = {};
  if (r < 30) { /* list */ }
  else if (r < 65) {
    // ~25% random GUIDs so not-found / error paths occur
    const id = t.ids.length && Math.random() > 0.25 ? pick(t.ids) : uuid();
    url += '/' + id;
  } else if (r < 90) {
    method = 'POST';
    init = { headers: { 'content-type': 'application/json' }, body: JSON.stringify(t.body()) };
  } else {
    method = 'DELETE';
    const id = t.ids.length && Math.random() > 0.25 ? t.ids.splice(rand(t.ids.length), 1)[0] : uuid();
    url += '/' + id;
  }
  stats.sent++;
  try {
    const res = await fetch(url, { ...init, method, signal: AbortSignal.timeout(15000) });
    await res.arrayBuffer();
    byStatus[res.status] = (byStatus[res.status] ?? 0) + 1;
    if (res.ok) {
      stats.ok++;
      if (method === 'POST') { t.ids.push(JSON.parse(init.body).id); if (t.ids.length > 200) t.ids.shift(); }
    } else stats.errors++;
  } catch {
    byStatus.network = (byStatus.network ?? 0) + 1;
    stats.errors++;
  }
}

const start = Date.now();
let stopping = false, last = { ...stats }, acc = 0;
const TICK = 100;

const tick = setInterval(() => {
  const t = (Date.now() - start) / 1000;
  if (duration && t >= duration) return stop();
  // sine wave (30s period, +-60%) plus +-20% noise, never negative
  const rate = Math.max(0, rps * (1 + 0.6 * Math.sin((2 * Math.PI * t) / 30) + (Math.random() - 0.5) * 0.4));
  acc += (rate * TICK) / 1000;
  for (; acc >= 1; acc--) { const p = fire().finally(() => inflight.delete(p)); inflight.add(p); }
}, TICK);

const report = setInterval(() => {
  const t = Math.round((Date.now() - start) / 1000);
  console.log(`[${String(t).padStart(4)}s] sent=${stats.sent} ok=${stats.ok} errors=${stats.errors} (+${stats.sent - last.sent} sent, +${stats.errors - last.errors} err in last 5s)`);
  last = { ...stats };
}, 5000);

async function stop() {
  if (stopping) return;
  stopping = true;
  clearInterval(tick); clearInterval(report);
  await Promise.allSettled(inflight);
  const secs = ((Date.now() - start) / 1000).toFixed(1);
  console.log(`\nDone in ${secs}s: sent=${stats.sent} ok=${stats.ok} errors=${stats.errors}`);
  console.log('Status counts:', JSON.stringify(byStatus));
  process.exit(0);
}
process.on('SIGINT', stop);
process.on('SIGTERM', stop);

console.log(`Sending ~${rps} rps to ${targets[0].base} and ${targets[1].base} for ${duration ? duration + 's' : 'until Ctrl-C'}`);
