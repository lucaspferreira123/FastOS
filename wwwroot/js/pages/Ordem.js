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

    AdicionarLinhaTabela(0, idProduto, nomeProduto, qtd, 0);
}

function AdicionarLinhaTabela(idItemOrdem, idProduto, nomeProduto, qtd, valorUnit) {

    let tr = `
        <tr data-id-item="${idItemOrdem}">
            <td data-id="${idProduto}">${nomeProduto}</td>
            <td><input type="number" class="form-control qtdPeca" min="1" value="${qtd}"></td>
            <td><input type="number" class="form-control valorPeca" min="0" step="0.01" value="${valorUnit}"></td>
            <td class="totalPeca">${(qtd * valorUnit).toFixed(2)}</td>
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

function GerarFuncoesPorStatus(o) {

    switch (o.statusDescricao) {
        case "Aguardando analise e requisição":
            return `
                <button class="btn btn-primary btn-sm" onclick="RequisitarItens(${o.idOrdemServico})">
                    Requisitar itens
                </button>

                <button class="btn btn-primary btn-sm" onclick="GerarOrcamento(${o.idOrdemServico})">
                    Gerar Orcamento
                </button>
            `;

        case "Em Andamento":
            return `
                <button class="btn btn-success btn-sm" onclick="Finalizar(${o.idOrdemServico})">
                    Finalizar
                </button>
            `;

        case "Aguardando Peça":
            return `
                <button class="btn btn-warning btn-sm" onclick="ComprarPeca(${o.idOrdemServico})">
                    Comprar Peça
                </button>
            `;

        case "Concluída":
            return `
                <button class="btn btn-info btn-sm" onclick="Visualizar(${o.idOrdemServico})">
                    Ver Detalhes
                </button>
            `;

        default:
            return "-";
    }
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
