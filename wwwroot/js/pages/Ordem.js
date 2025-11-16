
$(document).ready(function () {
    //const tabelaOS = $('#tabelaOS').DataTable({
    //    responsive: true,
    //    searching: true,
    //    paging: true,
    //    ordering: true,
    //    info: true,
    //    pageLength: 5,
    //    language: {
    //        url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/pt-BR.json"
    //    },
    //    columns: [
    //        { data: "idOrdemServico" },
    //        { data: "cliente" },
    //        { data: "servico" },
    //        { data: "dataAbertura" },
    //        { data: "previsaoEntrega" },
    //        { data: "status" },
    //        {
    //            data: null,
    //            className: "text-center",
    //            render: function (data) {
    //                return `
    //                    <button class="btn btn-warning btn-sm" onclick="EditarOS(${data.idOrdemServico})">
    //                        <i class="bi bi-pencil-square"></i>
    //                    </button>
    //                    <button class="btn btn-danger btn-sm" onclick="ExcluirOS(${data.idOrdemServico})">
    //                        <i class="bi bi-trash"></i>
    //                    </button>
    //                `;
    //            }
    //        }
    //    ]
    //});

    //ObterOrdens();
});

function ObterOrdens() {

    $.ajax({
        url: '/Ordem/ObterTodasOrdens',
        type: 'GET',
        success: function (response) {

            tabelaOS.clear();      
            tabelaOS.rows.add(response); 
            tabelaOS.draw();       
        },
        error: function (xhr) {
            console.error(xhr);
            Swal.fire("Erro!", "Não foi possível carregar as OS.", "error");
        }
    });
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

    if (!ordemBody.itens || ordemBody.itens.length === 0) {
        Swal.fire("Atenção!", "Adicione pelo menos um item à ordem de serviço.", "error");
        return false;
    }

    let item = ordemBody.itens[0];

    if (!item.idProduto || item.idProduto === 0) {
        Swal.fire("Atenção!", "Selecione um produto.", "error");
        return false;
    }

    if (!item.quantidade || item.quantidade <= 0) {
        Swal.fire("Atenção!", "Informe a quantidade do produto.", "error");
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
    let produto = $('#selectCriarOrdemProduto').val();
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
        itens: [
            {
                idItemOrdem: 0,
                idOrdemServico: 0,
                idProduto: parseInt(produto),
                dataPedido: dataAbertura,     
                dataRealizado: null,
                quantidade: 1
            }
        ]
    };

    return ordemJson;
}

