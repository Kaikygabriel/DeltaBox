🚀 O que é o DeltaBox?

O DeltaBox é um sistema de versionamento local desenvolvido em .NET, criado para registrar, comparar e restaurar versões de arquivos e diretórios de forma simples e eficiente.

Ele foi pensado para:

Evitar perda de arquivos recém-criados ou não salvos

Facilitar testes e experimentações sem medo de quebrar o projeto

Funcionar de forma consistente em Windows, Linux e macOS

🎯 Principais Objetivos

🔒 Segurança de versões: impedir perda de dados durante mudanças de versão

🔄 Versionamento incremental: acompanhar a evolução dos arquivos

🧩 Portabilidade total: funcionamento consistente em múltiplos sistemas operacionais

⚡ Simplicidade: uso direto via CLI, sem dependências externas complexas

✨ Funcionalidades

📂 Criar versões

Gera um snapshot do estado atual dos arquivos

🕒 Listar versões (commits)

Visualiza todo o histórico de versões criadas

🔍 Visualizar alterações

Mostra diferenças entre o estado atual e a última versão

Funciona de forma semelhante ao git status

⏪ Voltar para uma versão anterior

Restaura arquivos e pastas com segurança

🗑️ Remover versões

Exclui commits/versionamentos antigos quando não são mais necessários

🛡️ Proteção contra perda de arquivos

Arquivos recém-criados ou não salvos não são sobrescritos ao trocar de versão

🌍 Compatibilidade multiplataforma

Windows

Linux

macOS

🛠️ Tecnologias Utilizadas

.NET (C#)

Manipulação de arquivos e diretórios (System.IO)

Estrutura modular para evolução futura

CLI (Command Line Interface)
🧠 Aprendizados com o Projeto

Versionamento de arquivos na prática

Tratamento de paths multiplataforma (/ vs \)

Segurança em operações destrutivas

Design de ferramentas CLI

Pensamento arquitetural inspirado em sistemas reais (Git)

