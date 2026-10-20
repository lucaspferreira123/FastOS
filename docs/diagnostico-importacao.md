# Importação de diagnóstico do computador

O executável desktop (WPF/.NET) exporta o contrato JSON `schemaVersion: 1`. No formulário de criação ou edição da OS, use **Importar diagnóstico do computador (.json)**, selecione o arquivo, revise a prévia e marque as seções que deseja aplicar.

## Aplicação na OS

O modelo original da ordem não possuía campos de hardware. Foi adicionado o campo independente **Diagnóstico técnico do equipamento**, sem reutilizar a descrição do problema relatado pelo cliente. Os dados selecionados são acrescentados ao final do conteúdo já existente, após uma confirmação se o campo estiver preenchido. Cliente, descrição do problema, status, pagamento, valores, peças e demais campos da OS não são alterados pela importação.

Aplicar a prévia altera somente o formulário em memória. Revise os dados e use o botão normal de salvar a OS. Nesse salvamento, o JSON completo e o PDF opcional são associados à OS. A associação usa o ID da OS e nunca o nome do computador. Se o usuário fechar o formulário sem salvar, o arquivo pendente é descartado.

Diagnósticos associados aparecem no modal da OS. **Consultar dados** abre o JSON persistido; **Abrir PDF** baixa o relatório quando disponível. As rotas exigem autenticação e as operações de escrita exigem token antifalsificação.

## Validação e armazenamento

O FastOS aceita a versão de esquema 1, limita o JSON a 10 MB e o PDF a 20 MB, valida UTF-8, estrutura, GUID, data e assinatura `%PDF-`. O PDF recebe nome aleatório e fica em `App_Data/diagnostics`, fora de `wwwroot`; caminhos fornecidos no JSON não são utilizados. Um índice único no `DiagnosticId` impede importação duplicada, inclusive em concorrência.

## Migrações

- `20261012000000_AddDiagnosticImports`: tabela do diagnóstico completo, vínculo restritivo com a OS e índice único por identificador externo.
- `20261019000000_AddDiagnosticoTecnico`: campo independente para o texto técnico aplicado ao formulário.

As migrations não foram aplicadas ao banco. Após revisar e configurar a conexão do ambiente, aplique pelo procedimento normal, por exemplo:

```powershell
dotnet ef database update --project src/FastOS.Infrastructure --startup-project src/FastOS.API
```

Não atualize o banco de produção fora do processo de implantação do ambiente.
