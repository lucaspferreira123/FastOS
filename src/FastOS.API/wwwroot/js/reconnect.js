/**
 * FastOS — Detecção de queda do servidor e reconexão automática
 * Estratégia: polling no /health com backoff exponencial
 */
(function () {
    'use strict';

    // ── Configuração ────────────────────────────────────────────────────
    const HEALTH_URL        = '/health';
    const INTERVALO_INICIAL = 3000;   // 3s
    const INTERVALO_MAX     = 30000;  // 30s
    const FATOR_BACKOFF     = 2;

    // ── Estado ──────────────────────────────────────────────────────────
    let servidorOffline    = false;
    let tentativas         = 0;
    let intervaloAtual     = INTERVALO_INICIAL;
    let timerReconexao     = null;
    let timerContador      = null;
    let segundosProxTent   = 0;

    // ── Cria o banner (uma vez) ──────────────────────────────────────────
    const banner = document.createElement('div');
    banner.id = 'fastos-reconnect-banner';
    banner.innerHTML = `
        <div id="fastos-banner-offline" style="display:none">
            <span id="fastos-icon">⚠️</span>
            <span id="fastos-msg-principal"><strong>Servidor offline.</strong> Tentando reconectar...</span>
            <span id="fastos-tentativas"></span>
            <span id="fastos-countdown"></span>
            <span class="fastos-spinner"></span>
        </div>
        <div id="fastos-banner-online" style="display:none">
            ✅ <strong>Servidor reconectado!</strong> Recarregando a página...
        </div>
    `;

    const style = document.createElement('style');
    style.textContent = `
        #fastos-reconnect-banner {
            position: fixed;
            top: 0; left: 0; right: 0;
            z-index: 99999;
            font-family: Arial, sans-serif;
            font-size: 14px;
        }
        #fastos-banner-offline {
            background: #c0392b;
            color: #fff;
            padding: 12px 20px;
            display: flex;
            align-items: center;
            gap: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,.4);
            animation: fastos-slide-down .3s ease;
        }
        #fastos-banner-online {
            background: #1e8449;
            color: #fff;
            padding: 12px 20px;
            text-align: center;
            box-shadow: 0 2px 8px rgba(0,0,0,.4);
            animation: fastos-slide-down .3s ease;
        }
        #fastos-tentativas {
            background: rgba(255,255,255,.2);
            border-radius: 12px;
            padding: 2px 10px;
            font-size: 12px;
        }
        #fastos-countdown {
            font-size: 12px;
            opacity: .85;
        }
        .fastos-spinner {
            width: 16px; height: 16px;
            border: 2px solid rgba(255,255,255,.4);
            border-top-color: #fff;
            border-radius: 50%;
            animation: fastos-spin .8s linear infinite;
            margin-left: auto;
        }
        @keyframes fastos-spin {
            to { transform: rotate(360deg); }
        }
        @keyframes fastos-slide-down {
            from { transform: translateY(-100%); opacity: 0; }
            to   { transform: translateY(0);     opacity: 1; }
        }
    `;

    document.head.appendChild(style);
    document.body.prepend(banner);

    // ── Funções de UI ────────────────────────────────────────────────────
    function mostrarOffline() {
        document.getElementById('fastos-banner-online').style.display = 'none';
        document.getElementById('fastos-banner-offline').style.display = 'flex';
        document.getElementById('fastos-tentativas').textContent =
            `Tentativa ${tentativas}`;
    }

    function mostrarOnline() {
        document.getElementById('fastos-banner-offline').style.display = 'none';
        document.getElementById('fastos-banner-online').style.display = 'block';
    }

    function esconderBanner() {
        document.getElementById('fastos-banner-offline').style.display = 'none';
        document.getElementById('fastos-banner-online').style.display = 'none';
    }

    function atualizarContador(segundos) {
        const el = document.getElementById('fastos-countdown');
        if (el) el.textContent = segundos > 0
            ? `Próxima tentativa em ${segundos}s`
            : 'Tentando agora...';
    }

    // ── Lógica de polling ────────────────────────────────────────────────
    function verificarServidor() {
        fetch(HEALTH_URL, { method: 'GET', cache: 'no-store' })
            .then(res => {
                if (!res.ok) throw new Error('not ok');

                if (servidorOffline) {
                    // Servidor voltou!
                    servidorOffline = false;
                    clearTimeout(timerReconexao);
                    clearInterval(timerContador);
                    mostrarOnline();
                    setTimeout(() => location.reload(), 2000);
                } else {
                    esconderBanner();
                }

                // Reseta intervalo
                intervaloAtual = INTERVALO_INICIAL;
                tentativas = 0;
                agendarProximaVerificacao();
            })
            .catch(() => {
                if (!servidorOffline) {
                    servidorOffline = true;
                }

                tentativas++;
                mostrarOffline();

                // Backoff exponencial
                intervaloAtual = Math.min(intervaloAtual * FATOR_BACKOFF, INTERVALO_MAX);

                // Contador regressivo
                clearInterval(timerContador);
                segundosProxTent = Math.round(intervaloAtual / 1000);
                atualizarContador(segundosProxTent);

                timerContador = setInterval(() => {
                    segundosProxTent--;
                    atualizarContador(segundosProxTent);
                    if (segundosProxTent <= 0) clearInterval(timerContador);
                }, 1000);

                agendarProximaVerificacao();
            });
    }

    function agendarProximaVerificacao() {
        clearTimeout(timerReconexao);
        timerReconexao = setTimeout(verificarServidor, intervaloAtual);
    }

    // ── Inicia polling após 1s (deixa a página carregar primeiro) ────────
    // Não roda na página de login para não poluir
    const isLoginPage = window.location.pathname.toLowerCase().includes('login');
    if (!isLoginPage) {
        setTimeout(verificarServidor, 1000);
    }

})();
