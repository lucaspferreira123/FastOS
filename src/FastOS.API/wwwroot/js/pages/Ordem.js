$(document).ready(function () {
    ObterOrdens();
    InicializarFiltros();
});

tbody = $("#tabelaOrdens tbody");

tabelaOrdensDT = $('#tabelaOrdens').DataTable({
    language: {
        search: "Buscar: ",
        lengthMenu: "Mostrando _MENU_ itens por página",
        zeroRecords: "Nenhum Registro Encontrado",
        info: "Mostrando página _PAGE_ de _PAGES_",
    }
});

function ObterInstanciaModal(seletor) {
    const elemento = document.querySelector(seletor);

    if (!elemento) {
        return null;
    }

    if (window.bootstrap?.Modal) {
        return bootstrap.Modal.getOrCreateInstance(elemento);
    }

    return null;
}

function AbrirModal(seletor) {
    const modal = ObterInstanciaModal(seletor);

    if (modal) {
        modal.show();
        return;
    }

    $(seletor).modal("show");
}

function FecharModal(seletor) {
    const modal = ObterInstanciaModal(seletor);

    if (modal) {
        modal.hide();
        return;
    }

    $(seletor).modal("hide");
}

function ObterOrdens() {

    $.ajax({
        url: '/Ordem/ObterOrdens',
        type: 'GET',
        success: function (ordens) {

            todasOrdensCache = ordens;
            PopularSelectsFiltro(ordens);

            tabelaOrdensDT.clear();

            ordens.forEach(o => {

                let funcoes = GerarFuncoesPorStatus(o);

                tabelaOrdensDT.row.add([
                    o.idOrdemServico,
                    `<a href="/Cliente/Perfil/${o.idCliente}" class="text-decoration-none fw-semibold text-danger" title="Ver perfil do cliente">${o.clienteNome}</a>`,
                    o.pago ? "Sim" : "Não",
                    o.statusDescricao,
                    formatarData(o.dataAbertura),
                    formatarData(o.previsaoEntrega),
                    funcoes
                ]);
            });

            tabelaOrdensDT.draw();
        }
    });
}

function AlterarStatus(idOrdem, idStatus) {
    if (idStatus == null)
        return;

    $.ajax({
        url: `/Ordem/AlterarStatusOrdem/${idOrdem}/${idStatus}`,
        type: 'PUT',
        success: function (itens) {

            if (idStatus == 3)
                Swal.fire("Sucesso!", "Requisição e analise finalizadas com sucesso!", "success");
            else if (idStatus == 4)
                Swal.fire("Sucesso!", "Orçamento enviado para aguardando aprovação com sucesso!", "success");
            else if (idStatus == 7)
                Swal.fire("Sucesso!", "Orçamento aprovado e a execução da ordem foi iniciada com sucesso!", "success");
            else if (idStatus == 8)
                Swal.fire("Sucesso!", "Ordem finalizada com sucesso", "success");
            else if (idStatus == 5)
                Swal.fire("Sucesso!", "Ordem finalizada com sucesso", "success");
            ObterOrdens();
        },
        error: function () {
        }
    });
}

function RequisitarItens(idOrdemServico) {

    AbrirModal("#modalAdicionarPecas");

    $("#modalAdicionarPecas").attr("data-id-os", idOrdemServico);

    CarregarItensDaOrdem(idOrdemServico);
}

function CarregarItensDaOrdem(idOrdemServico) {

    $.ajax({
        url: `/ItemOrdemServico/ObterItensOrdemServico/${idOrdemServico}`,
        type: 'GET',
        success: function (itens) {

            let tbody = $("#listaPecas");
            tbody.empty();

            itens.forEach(i => {
                AdicionarLinhaTabela(i.idItemOrdemServico, i.idProduto, i.nomeProduto, i.quantidade, i.valorUnitario);
            });
        },
        error: function () {
        }
    });
}

function AdicionarPeca() {
    let idProduto = $("#selectCriarOrdemProduto").val();
    let nomeProduto = $("#selectCriarOrdemProduto option:selected").text();
    let qtd = $("#txtQuantidadePeca").val();

    if (!idProduto) {
        Swal.fire("Atenção", "Selecione um produto.", "warning");
        return;
    }

    AdicionarLinhaTabela(0, idProduto, nomeProduto, qtd);
}

function AdicionarLinhaTabela(idItemOrdem, idProduto, nomeProduto, qtd) {

    let tr = `
    <tr data-id-item="${idItemOrdem}">
        <td>${idProduto}</td> <!-- Código do produto -->
        <td data-id="${idProduto}">${nomeProduto}</td>
        <td>
            <input type="number" class="form-control qtdPeca" min="1" value="${qtd}">
        </td>
        <td>
            <button class="btn btn-danger btn-sm btnRemover">X</button>
        </td>
    </tr>
`;

    $("#listaPecas").append(tr);

    AtualizarTotais();
}

$(document).on("input", ".qtdPeca, .valorPeca", function () {
    AtualizarTotais();
});

$(document).on("click", ".btnRemover", function () {
    $(this).closest("tr").remove();
    AtualizarTotais();
});

function AtualizarTotais() {

    $("#listaPecas tr").each(function () {

        let qtd = parseFloat($(this).find(".qtdPeca").val()) || 0;
        let valor = parseFloat($(this).find(".valorPeca").val()) || 0;

        let total = qtd * valor;

        $(this).find(".totalPeca").text(total.toFixed(2));
    });
}

function SalvarItens() {

    var itens = CarregarJsonItensOrdem();

    $.ajax({
        url: `/ItemOrdemServico/AlterarItensOrdemServico`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(itens),
        success: function () {
            Swal.fire("Sucesso!", "Itens salvos com sucesso!", "success");
            $("#modalAdicionarPecas").modal("hide");
            ObterOrdens();
        },
        error: function (xhr) {
            let msg = "Não foi possível salvar os itens da ordem.";
            if (xhr.status === 400 && xhr.responseText) {
                try {
                    // tenta parsear se vier como JSON string
                    msg = JSON.parse(xhr.responseText);
                } catch {
                    msg = xhr.responseText;
                }
                // remove aspas extras se vier como "mensagem"
                msg = msg.replace(/^"|"$/g, '');
            }
            Swal.fire({
                icon: "error",
                title: "Estoque insuficiente",
                text: msg,
                confirmButtonColor: "#dc3545"
            });
        }
    });
}

function CarregarJsonItensOrdem() {

    let idOS = parseInt($("#modalAdicionarPecas").attr("data-id-os"));
    let pecas = [];

    $("#listaPecas tr").each(function () {

        let idItemAttr = $(this).attr("data-id-item");
        let idItem = idItemAttr ? parseInt(idItemAttr) : null;

        let idProduto = parseInt($(this).find("td[data-id]").attr("data-id"));
        let quantidade = parseInt($(this).find(".qtdPeca").val());

        pecas.push({
            idItemOrdem: idItem,
            idOrdemServico: idOS,
            idProduto: idProduto,
            dataPedido: new Date().toISOString(),
            dataRealizado: null,
            quantidade: quantidade
        });
    });

    return pecas;
}


function LimparCamposAlterarOrcamento() {
    $("#txtOrcamentoMaoDeObra").val("");
    $("#txtOrcamentoMateriais").val("");
    $("#txtOrcamentoDesconto").val("");
    $("#txtOrcamentoTaxas").val("");
    $("#txtOrcamentoValorFinal").val("");
    $("#txtOrcamentoFormaPagamento").val("");
}

function GerarFuncoesPorStatus(o) {

    switch (o.statusDescricao) {

        case "Aguardando Analise e Requisição":
            return `
                <div class="d-flex gap-2 flex-wrap">
                    <button class="btn btn-outline-primary btn-sm" title="Requisitar Itens"
                        onclick="RequisitarItens(${o.idOrdemServico})">
                        <i class="bi bi-box-arrow-in-down"></i>
                    </button>

                    <button class="btn btn-outline-secondary btn-sm" title="Alterar Ordem"
                        onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
                        <i class="bi bi-pencil"></i>
                    </button>

                    <button class="btn btn-outline-success btn-sm" title="Finalizar Análise"
                        onclick="AlterarStatus(${o.idOrdemServico}, 2)">
                        <i class="bi bi-check2-circle"></i>
                    </button>
                </div>
            `;

        case "Gerando Orcamento":
            return `
                <div class="d-flex gap-2 flex-wrap">
                    <button class="btn btn-outline-secondary btn-sm" title="Alterar Ordem"
                        onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
                        <i class="bi bi-pencil"></i>
                    </button>

                    <button class="btn btn-outline-primary btn-sm" title="Alterar Orçamento"
                        onclick="AbrirModalAlterarOrcamento(${o.idOrdemServico})">
                        <i class="bi bi-calculator"></i>
                    </button>

                    <button class="btn btn-outline-dark btn-sm" title="Imprimir Orçamento"
                        onclick="ImprimirOrcamento(${o.idOrdemServico})">
                        <i class="bi bi-printer"></i>
                    </button>

                    <button class="btn btn-outline-success btn-sm" title="Enviar para Aprovação"
                        onclick="AlterarStatus(${o.idOrdemServico}, 3)">
                        <i class="bi bi-send-check"></i>
                    </button>
                </div>
            `;

        case "Aguardando Aprovação":
            return `
                <div class="d-flex gap-2 flex-wrap">
                    <button class="btn btn-outline-secondary btn-sm" title="Alterar Ordem"
                        onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
                        <i class="bi bi-pencil"></i>
                    </button>

                    <button class="btn btn-outline-success btn-sm" title="Aprovar Orçamento"
                        onclick="AlterarStatus(${o.idOrdemServico}, 4)">
                        <i class="bi bi-check-circle"></i>
                    </button>
                </div>
            `;

        case "Ordem em Execução":
            return `
                <div class="d-flex gap-2 flex-wrap">
                    <button class="btn btn-outline-secondary btn-sm" title="Alterar Ordem"
                        onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
                        <i class="bi bi-pencil"></i>
                    </button>

                    <button class="btn btn-outline-success btn-sm" title="Finalizar Ordem"
                        onclick="AlterarStatus(${o.idOrdemServico}, 5)">
                        <i class="bi bi-check2-circle"></i>
                    </button>
                </div>
            `;

        case "Concluída / Aguardando Pagamento":
            return `
                <div class="d-flex gap-2 flex-wrap">
                    <button class="btn btn-outline-secondary btn-sm" title="Alterar Ordem"
                        onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
                        <i class="bi bi-pencil"></i>
                    </button>

                    <button class="btn btn-outline-dark btn-sm" title="Imprimir Recibo"
                        onclick="ImprimirBoleto(${o.idOrdemServico})">
                        <i class="bi bi-receipt"></i>
                    </button>

                    <button class="btn btn-outline-info btn-sm" title="Enviar Recibo por E-mail"
                        onclick="EnviarReciboPorEmail(${o.idOrdemServico})">
                        <i class="bi bi-envelope"></i>
                    </button>

                    <button class="btn btn-outline-success btn-sm" title="Confirmar Pagamento"
                        onclick="AlterarStatus(${o.idOrdemServico}, 6)">
                        <i class="bi bi-cash-coin"></i>
                    </button>
                </div>
            `;

        case "Concluída / Pagamento Realizado":
        case "Cancelada":
            return `
                <div class="d-flex gap-2 flex-wrap">
                    <button class="btn btn-outline-secondary btn-sm" title="Alterar Ordem"
                        onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
                        <i class="bi bi-pencil"></i>
                    </button>
                </div>
            `;

        default:
            return "-";
    }
}

function AbrirModalAlterarOrdem(idOrdem) {

    AbrirModal("#modalEditarOS");

    $("#modalEditarOS").data("id-ordem", idOrdem);

    LimparCamposAlterarOrdem();

    $.ajax({
        url: `/Ordem/ObterOrdem/${idOrdem}`,
        type: 'GET',
        success: function (ordem) {

            $("#selectEditarOrdemCliente").val(ordem.idCliente);
            $("#txtEditarOrdemDataAbertura").val(ordem.dataAbertura?.substring(0, 10));
            $("#txtEditarOrdemPrevisaoEntrega").val(ordem.previsaoEntrega?.substring(0, 10));
            $("#selectEditarOrdemStatus").val(ordem.idStatus);
            $("#txtEditarOrdemDescricao").val(ordem.descricaoServico);
        },
        error: function () {
            console.error("Erro ao obter a ordem.");
        }
    });
}

function AlterarOrdem() {

    const idOrdem = $("#modalEditarOS").data("id-ordem");

    var ordemAlterada = CarregarJsonOrdemEditar(idOrdem);

    var ordemValidada = ValidarCamposCriarOrdem(ordemAlterada);

    if (ordemValidada) {
        $.ajax({
            url: '/Ordem/AlterarOrdem',
            type: 'PUT',
            data: JSON.stringify(ordemAlterada),
            contentType: 'application/json; charset=utf-8',
            dataType: 'text',
            success: function (response) {

                Swal.fire({
                    icon: "success",
                    title: "Sucesso!",
                    text: `Ordem de serviço alterada com sucesso!`,
                });
                ObterOrdens();
            },
            error: function (xhr) {
                Swal.fire("Erro!", "Não foi possível alterar a OS.", "error");
            }
        });
    }
}

function CarregarJsonOrdemEditar(idOrdem) {
    let cliente = $('#selectEditarOrdemCliente').val();
    let previsaoEntrega = $('#txtEditarOrdemPrevisaoEntrega').val();
    let dataAbertura = $('#txtEditarOrdemDataAbertura').val();
    let descricao = $('#txtEditarOrdemDescricao').val();
    let status = $('#selectEditarOrdemStatus').val();

    let ordemJson = {
        idOrdemServico: idOrdem,
        idCliente: parseInt(cliente),
        pago: false,
        idStatus: parseInt(status),
        descricaoServico: descricao,
        dataAbertura: dataAbertura,
        previsaoEntrega: previsaoEntrega,
    };

    return ordemJson;
}

function LimparCamposAlterarOrdem() {
    $("#selectEditarOrdemCliente").val("");
    $("#txtEditarOrdemDataAbertura").val("");
    $("#txtEditarOrdemPrevisaoEntrega").val("");
    $("#selectEditarOrdemStatus").val("");
    $("#txtEditarOrdemDescricao").val("");
}

function formatarData(data) {
    if (!data) return "";
    return new Date(data).toLocaleDateString("pt-BR");
}

function ValidarCamposCriarOrdem(ordemBody) {

    if (!ordemBody.idCliente || ordemBody.idCliente === 0) {
        Swal.fire("Atenção!", "Selecione um cliente.", "error");
        return false;
    }

    if (!ordemBody.descricaoServico || ordemBody.descricaoServico === 0) {
        Swal.fire("Atenção!", "Preencha a descrição do serviço.", "error");
        return false;
    }

    if (!ordemBody.idStatus || ordemBody.idStatus === 0) {
        Swal.fire("Atenção!", "Selecione o status da ordem.", "error");
        return false;
    }

    if (!ordemBody.dataAbertura || ordemBody.dataAbertura.trim() === "") {
        Swal.fire("Atenção!", "Preencha a data de abertura.", "error");
        return false;
    }

    if (!ordemBody.previsaoEntrega || ordemBody.previsaoEntrega.trim() === "") {
        Swal.fire("Atenção!", "Preencha a previsão de entrega.", "error");
        return false;
    }

    return true;
}

function CriarOrdem() {

    var ordemBody = CarregarJsonOrdem();

    let ordemValidada = ValidarCamposCriarOrdem(ordemBody);

    if (ordemValidada) {
        $.ajax({
            url: '/Ordem/CadastrarOrdem',
            type: 'POST',
            data: JSON.stringify(ordemBody),
            contentType: 'application/json; charset=utf-8',
            dataType: 'text',
            success: function (response) {

                FecharModal("#modalCadastroOS");

                let ordem = JSON.parse(response);

                Swal.fire({
                    icon: "success",
                    title: "Sucesso!",
                    text: `Ordem de serviço Nº: ${ordem.id} criada com sucesso!`,
                });

                LimparInputsCriarOrdem();
                ObterOrdens();
            },
            error: function (xhr) {
                Swal.fire("Erro!", "Não foi possível carregar as OS.", "error");
            }
        });
    }
}

function CarregarJsonOrdem() {
    let cliente = $('#selectCriarOrdemCliente').val();
    let previsaoEntrega = $('#txtCriarOrdemPrevisaoEntrega').val();
    let dataAbertura = $('#txtCriarOrdemDataAbertura').val();
    let descricao = $('#txtCriarOrdemDescricao').val();
    let status = $('#selectCriarOrdemStatus').val();

    let ordemJson = {
        idOrdemServico: 0,
        idCliente: parseInt(cliente),
        pago: false,
        idStatus: parseInt(status),
        descricaoServico: descricao,
        dataAbertura: dataAbertura,
        previsaoEntrega: previsaoEntrega,
    };

    return ordemJson;
}

function LimparInputsCriarOrdem() {
    $('#selectCriarOrdemCliente').val('');
    $('#txtCriarOrdemPrevisaoEntrega').val('');
    $('#txtCriarOrdemDataAbertura').val('');
    $('#txtCriarOrdemDescricao').val('');
    $('#selectCriarOrdemStatus').val('');
}

function SalvarOrcamento() {

    var idOrdem = $("#modalOrcamentoOS").data("id-ordem");
    var objetoOrcamento = CarregarJsonOrcamento(idOrdem);

    $.ajax({
        url: '/Orcamento/AlterarOrcamento',
        type: 'POST',
        data: JSON.stringify(objetoOrcamento),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {

            FecharModal("#modalOrcamentoOS");

            Swal.fire({
                icon: "success",
                title: "Sucesso!",
                text: `Orcamento salvo com sucesso!`,
            });
        },
        error: function (xhr) {
            Swal.fire("Erro!", "Não foi possível salvar o orçamento.", "error");
        }
    });
}

function CarregarJsonOrcamento(idOrdem) {

    const json = {
        idOrdemServico: idOrdem,
        maoDeObra: parseFloat($("#txtOrcamentoMaoDeObra").val()) || 0,
        materiais: parseFloat($("#txtOrcamentoMateriais").val()) || 0,
        desconto: parseFloat($("#txtOrcamentoDesconto").val()) || 0,
        taxasExtras: parseFloat($("#txtOrcamentoTaxas").val()) || 0,
        formaPagamento: $("#txtOrcamentoFormaPagamento").val() || "",
        valorFinal: parseFloat($("#txtOrcamentoValorFinal").val()) || 0
    };

    return json;
}

function AbrirModalAlterarOrcamento(idOrdem) {
    AbrirModal("#modalOrcamentoOS");
    $("#modalOrcamentoOS").data("id-ordem", idOrdem);

    LimparCamposAlterarOrcamento();
    ObterOrcamento(idOrdem);

}

function ObterOrcamento(idOrdem) {
    $.ajax({
        url: `/Orcamento/ObterOrcamento/${idOrdem}`,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response) {
                $("#txtOrcamentoMaoDeObra").val(response.maoDeObra);
                $("#txtOrcamentoDesconto").val(response.desconto);
                $("#txtOrcamentoTaxas").val(response.taxasExtras);
                $("#txtOrcamentoFormaPagamento").val(response.formaPagamento);
                $("#txtOrcamentoValorFinal").val(response.valorFinal);
            }

            PopularValorTotalPecas(idOrdem);
        },
        error: function (xhr) {
            PopularValorTotalPecas(idOrdem);
        }
    });
}

function PopularValorTotalPecas(idOrdem) {

    $.ajax({
        url: `/ItemOrdemServico/ObterItensOrdemServico/${idOrdem}`,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            let totalMateriais = 0;

            $.each(response, function (i, item) {
                let valor = parseFloat(item.valorUnitario ?? item.valorVenda ?? item.valor ?? 0) || 0;
                let quantidade = parseFloat(item.quantidade) || 0;

                totalMateriais += valor * quantidade;
            });

            $("#txtOrcamentoMateriais").val(totalMateriais.toFixed(2));
            CalcularValorOrcamento();
        },
        error: function (xhr) {
            Swal.fire("Erro!", "Não foi possível salvar o orçamento.", "error");
        }
    });
}

function CalcularValorOrcamento() {

    let maoDeObra = parseFloat($("#txtOrcamentoMaoDeObra").val()) || 0;
    let materiais = parseFloat($("#txtOrcamentoMateriais").val()) || 0;
    let desconto = parseFloat($("#txtOrcamentoDesconto").val()) || 0;
    let taxas = parseFloat($("#txtOrcamentoTaxas").val()) || 0;

    // Soma base
    let valorBase = maoDeObra + materiais;

    // Aplica desconto (%)
    let valorComDesconto = valorBase - (valorBase * (desconto / 100));

    // Soma taxas extras
    let valorFinal = valorComDesconto + taxas;

    $("#txtOrcamentoValorFinal").val(valorFinal.toFixed(2));
}

function ImprimirOrcamento(idOrdem) {
    const url = `/Relatorio/ImprimirRelatorioOrcamento/${idOrdem}`;
    window.open(url, '_blank');
}

function ImprimirBoleto(idOrdem) {
    const url = `/Relatorio/ImprimirRecibo/${idOrdem}`;
    window.open(url, '_blank');
}

function EnviarReciboPorEmail(idOrdem) {
    Swal.fire({
        title: 'Enviar Recibo',
        text: 'Deseja enviar o recibo por e-mail para o cliente?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#0dcaf0',
        cancelButtonColor: '#6c757d',
        confirmButtonText: '<i class="bi bi-envelope"></i> Enviar',
        cancelButtonText: 'Cancelar'
    }).then(result => {
        if (!result.isConfirmed) return;

        Swal.fire({
            title: 'Enviando...',
            text: 'Aguarde enquanto o e-mail é enviado.',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });

        $.ajax({
            url: `/Relatorio/EnviarReciboPorEmail/${idOrdem}`,
            type: 'POST',
            success: function (response) {
                Swal.fire('Enviado!', response.mensagem, 'success');
            },
            error: function (xhr) {
                const msg = xhr.responseText || 'Não foi possível enviar o e-mail.';
                Swal.fire('Erro!', msg, 'error');
            }
        });
    });
}

// ── Filtros avançados ────────────────────────────────────────────────────

function InicializarFiltros() {
    // Injeta a barra de filtros antes da tabela
    const barraHtml = `
    <div id="barraFiltros" class="card border-0 shadow-sm mb-3 mx-3">
        <div class="card-body py-2 px-3">
            <div class="row g-2 align-items-end">
                <div class="col-12 col-md-3">
                    <label class="form-label small fw-semibold mb-1">Status</label>
                    <select id="filtroStatus" class="form-select form-select-sm">
                        <option value="">Todos os status</option>
                    </select>
                </div>
                <div class="col-12 col-md-3">
                    <label class="form-label small fw-semibold mb-1">Cliente</label>
                    <select id="filtroCliente" class="form-select form-select-sm">
                        <option value="">Todos os clientes</option>
                    </select>
                </div>
                <div class="col-6 col-md-2">
                    <label class="form-label small fw-semibold mb-1">Pago</label>
                    <select id="filtroPago" class="form-select form-select-sm">
                        <option value="">Todos</option>
                        <option value="Sim">Sim</option>
                        <option value="Não">Não</option>
                    </select>
                </div>
                <div class="col-6 col-md-2">
                    <label class="form-label small fw-semibold mb-1">Abertura de</label>
                    <input type="date" id="filtroDataDe" class="form-control form-control-sm">
                </div>
                <div class="col-6 col-md-2">
                    <label class="form-label small fw-semibold mb-1">Até</label>
                    <input type="date" id="filtroDataAte" class="form-control form-control-sm">
                </div>
                <div class="col-6 col-md-auto d-flex gap-2 align-items-end">
                    <button class="btn btn-danger btn-sm" onclick="AplicarFiltros()">
                        <i class="bi bi-funnel-fill"></i> Filtrar
                    </button>
                    <button class="btn btn-outline-secondary btn-sm" onclick="LimparFiltros()" title="Limpar filtros">
                        <i class="bi bi-x-lg"></i>
                    </button>
                </div>
            </div>
        </div>
    </div>`;

    $('#tabelaOrdens').closest('.table-responsive').before(barraHtml);
}

function PopularSelectsFiltro(ordens) {
    // Status únicos
    const statusSet = [...new Set(ordens.map(o => o.statusDescricao))].sort();
    const selStatus = $('#filtroStatus');
    const valStatus = selStatus.val();
    selStatus.find('option:not(:first)').remove();
    statusSet.forEach(s => selStatus.append(`<option value="${s}">${s}</option>`));
    if (valStatus) selStatus.val(valStatus);

    // Clientes únicos
    const clienteSet = [...new Set(ordens.map(o => o.clienteNome))].sort();
    const selCliente = $('#filtroCliente');
    const valCliente = selCliente.val();
    selCliente.find('option:not(:first)').remove();
    clienteSet.forEach(c => selCliente.append(`<option value="${c}">${c}</option>`));
    if (valCliente) selCliente.val(valCliente);
}

// Guarda todas as ordens para filtrar client-side
let todasOrdensCache = [];

function AplicarFiltros() {
    const status   = $('#filtroStatus').val().toLowerCase();
    const cliente  = $('#filtroCliente').val().toLowerCase();
    const pago     = $('#filtroPago').val().toLowerCase();
    const dataDe   = $('#filtroDataDe').val();
    const dataAte  = $('#filtroDataAte').val();

    tabelaOrdensDT.clear();

    const filtradas = todasOrdensCache.filter(o => {
        if (status  && o.statusDescricao.toLowerCase() !== status)  return false;
        if (cliente && o.clienteNome.toLowerCase()     !== cliente) return false;
        if (pago === 'sim' && !o.pago)  return false;
        if (pago === 'não' && o.pago)   return false;

        if (dataDe || dataAte) {
            const abertura = new Date(o.dataAbertura);
            if (dataDe && abertura < new Date(dataDe)) return false;
            if (dataAte && abertura > new Date(dataAte + 'T23:59:59')) return false;
        }

        return true;
    });

    filtradas.forEach(o => {
        tabelaOrdensDT.row.add([
            o.idOrdemServico,
            `<a href="/Cliente/Perfil/${o.idCliente}" class="text-decoration-none fw-semibold text-danger" title="Ver perfil do cliente">${o.clienteNome}</a>`,
            o.pago ? "Sim" : "Não",
            o.statusDescricao,
            formatarData(o.dataAbertura),
            formatarData(o.previsaoEntrega),
            GerarFuncoesPorStatus(o)
        ]);
    });

    tabelaOrdensDT.draw();

    // Badge com total filtrado
    const total = filtradas.length;
    const badge = total < todasOrdensCache.length
        ? `<span class="badge bg-danger ms-2">${total} resultado${total !== 1 ? 's' : ''}</span>`
        : '';
    $('#badgeFiltro').remove();
    if (badge) $('#barraFiltros').after(`<div id="badgeFiltro" class="mx-3 mb-2">${badge}</div>`);
}

function LimparFiltros() {
    $('#filtroStatus').val('');
    $('#filtroCliente').val('');
    $('#filtroPago').val('');
    $('#filtroDataDe').val('');
    $('#filtroDataAte').val('');
    $('#badgeFiltro').remove();
    AplicarFiltros();
}
