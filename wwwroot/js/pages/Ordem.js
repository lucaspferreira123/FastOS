$(document).ready(function () {

    // 🚨 Troque #ID_DO_SEU_OFFCANVAS pelo ID real do seu offcanvas
    const offcanvasSeletor = '#offcanvasSidebar';


    ObterOrdens();
});

function ObterOrdens() {

    $.ajax({
        url: '/Ordem/ObterOrdens',
        type: 'GET',
        success: function (ordens) {

            let tbody = $("#tabelaOrdens tbody");

            tbody.empty();

            ordens.forEach(o => {

                let funcoes = GerarFuncoesPorStatus(o);

                let linha = `
                    <tr>
                        <td>${o.idOrdemServico}</td>
                        <td>${o.clienteNome}</td>
                        <td>${o.pago ? "Sim" : "Não"}</td>
                        <td>${o.statusDescricao}</td>
                        <td>${formatarData(o.dataAbertura)}</td>
                        <td>${formatarData(o.previsaoEntrega)}</td>
                        <td>${funcoes}</td>
                    </tr>
                `;

                tbody.append(linha);
            });

            let test = new DataTable('#tabelaOrdens', {
                language: {
                    search: "Buscar: ",
                    lengthMenu: "Mostrando _MENU_ itens por página",
                    zeroRecords: "Nenhum Registro Encontrado",
                    info: "Mostrando pagina _PAGE_ de _PAGES_",
                }
            });
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

    $("#modalAdicionarPecas").modal("show");

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
        },
        error: function () {
            Swal.fire("Erro!", "Não foi possível salvar os itens da ordem.", "error");
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
                        onclick="AlterarStatus(${o.idOrdemServico}, 3)">
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
                        onclick="AlterarStatus(${o.idOrdemServico}, 4)">
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
                        onclick="AlterarStatus(${o.idOrdemServico}, 7)">
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
                        onclick="AlterarStatus(${o.idOrdemServico}, 8)">
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

                    <button class="btn btn-outline-dark btn-sm" title="Imprimir Boleto"
                        onclick="ImprimirBoleto(${o.idOrdemServico})">
                        <i class="bi bi-receipt"></i>
                    </button>

                    <button class="btn btn-outline-success btn-sm" title="Confirmar Pagamento"
                        onclick="AlterarStatus(${o.idOrdemServico}, 5)">
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



//function GerarFuncoesPorStatus(o) {

//    switch (o.statusDescricao) {
//        case "Aguardando Analise e Requisição":
//            return `
//                <button class="btn btn-primary btn-sm" onclick="RequisitarItens(${o.idOrdemServico})">
//                    Requisitar itens
//                </button>

//                <button class="btn btn-primary btn-sm" onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
//                    Alterar Ordem
//                </button>

//                <button class="btn btn-primary btn-sm" onclick="AlterarStatus(${o.idOrdemServico}, 3)">
//                    Finalizar Analise e Requisição
//                </button>
//            `;

//        case "Gerando Orcamento":
//            return `
//                <button class="btn btn-primary btn-sm" onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
//                    Alterar Ordem
//                </button>

//                <button class="btn btn-primary btn-sm" onclick="AbrirModalAlterarOrcamento(${o.idOrdemServico})">
//                    Alterar Orçamento
//                </button>

//                 <button class="btn btn-primary btn-sm" onclick="ImprimirOrcamento(${o.idOrdemServico})">
//                    Imprimir Orçamento
//                </button>

//                 <button class="btn btn-primary btn-sm" onclick="AlterarStatus(${o.idOrdemServico}, 4)">
//                    Enviar para aguardando aprovação
//                </button>
//            `;

//        case "Aguardando Aprovação":
//            return `
//                <button class="btn btn-primary btn-sm" onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
//                    Alterar Ordem
//                </button>

//                <button class="btn btn-primary btn-sm" onclick="AlterarStatus(${o.idOrdemServico}, 7)">
//                    Aprovar Orçamento Ordem
//                </button>
//            `;

//        case "Ordem em Execução":
//            return `
//                <button class="btn btn-primary btn-sm" onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
//                    Alterar Ordem
//                </button>

//                <button class="btn btn-primary btn-sm" onclick=AlterarStatus"(${o.idOrdemServico}, 8)">
//                    Finalizar Ordem
//                </button>
//            `;

//        case "Concluída / Aguardando Pagamento":
//            return `
//                <button class="btn btn-primary btn-sm" onclick="AlterarOrdem(${o.idOrdemServico})">
//                    Alterar Ordem
//                </button>

//                <button class="btn btn-primary btn-sm" onclick="ImprimirBoleto(${o.idOrdemServico})">
//                    Imprimir Boleto
//                </button>

//                <button class="btn btn-primary btn-sm" onclick="AlterarStatus(${o.idOrdemServico}, 5)">
//                    Confirmar Pagamento
//                </button>
//            `;

//        case "Concluída / Pagamento Realizado":
//            return `
//                <button class="btn btn-primary btn-sm" onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
//                    Alterar Ordem
//                </button>
//            `;

//        case "Cancelada":
//            return `
//                <button class="btn btn-primary btn-sm" onclick="AbrirModalAlterarOrdem(${o.idOrdemServico})">
//                    Alterar Ordem
//                </button>
//            `;

//        default:
//            return "-";
//    }
//}

function AbrirModalAlterarOrdem(idOrdem) {

    $("#modalEditarOS").modal("show");

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

                let ordem = JSON.parse(response);

                Swal.fire({
                    icon: "success",
                    title: "Sucesso!",
                    text: `Ordem de serviço Nº: ${ordem.id} criada com sucesso!`,
                });

                LimparInputsCriarOrdem();
                ObterOrdens();
                $('#modalCadastroOS').modal('hide');
                $('.modal-backdrop').remove();
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

            Swal.fire({
                icon: "success",
                title: "Sucesso!",
                text: `Orcamento salvo com sucesso!`,
            });

            $('#modalOrcamentoOS').modal('hide');
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
    $("#modalOrcamentoOS").modal("show");
    $("#modalOrcamentoOS").data("id-ordem", idOrdem);

    LimparCamposAlterarOrcamento();

    var orcamento = ObterOrcamento(idOrdem);

    if (orcamento == null || orcamento == undefined) {
        PopularValorTotalPecas(idOrdem);
    }

}

function ObterOrcamento(idOrdem) {


    $.ajax({
        url: `/Orcamento/ObterOrcamento/${idOrdem}`,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            console.log(response)

            $("#txtOrcamentoMaoDeObra").val(response.maoDeObra);
            $("#txtOrcamentoMateriais").val(response.materiais);
            $("#txtOrcamentoDesconto").val(response.desconto);
            $("#txtOrcamentoValorFinal").val(response.formaPagamento);
            $("#txtOrcamentoFormaPagamento").val(response.valorFinal);
            $("#txtOrcamentoTaxas").val(response.taxasExtras);
        },
        error: function (xhr) {
            
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
                let valor = parseFloat(item.valorUnitario) || 0;
                let quantidade = parseFloat(item.quantidade) || 0;

                totalMateriais += valor * quantidade;
            });

            $("#txtOrcamentoMateriais").val(totalMateriais.toFixed(2));
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
