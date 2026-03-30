# 🚀 DeltaBox

## 📖 O que é o DeltaBox?

O **DeltaBox** é um sistema de versionamento local desenvolvido em .NET, criado para registrar, comparar e restaurar versões de arquivos e diretórios de forma simples e eficiente.

Ele foi projetado para:

* Versionamento de arquivos (Similar ao git)
* Evitar perda de arquivos recém-criados ou não salvos
* Facilitar testes e experimentações sem risco de quebrar o projeto
* Funcionar de forma consistente em **Windows, Linux e macOS**

---

## 🎯 Objetivos

* 🔒 **Segurança de versões**
  Impedir perda de dados durante mudanças de versão

* 🔄 **Versionamento incremental**
  Acompanhar a evolução dos arquivos ao longo do tempo

* 🌍 **Portabilidade**
  Funcionamento consistente em múltiplos sistemas operacionais

* ⚡ **Simplicidade**
  Uso direto via CLI, sem dependências externas complexas

---

## ✨ Funcionalidades

### 📂 Criação de versões

Gera um snapshot do estado atual dos arquivos.

---

### 📂 Compatibilidade com GitHub

Pode se usar o github junto ao deltabox para alocação de arquivos remotos.

---

### 🕒 Histórico de versões

Lista todos os commits/versionamentos criados.

---

### 🔍 Visualização de alterações

Mostra as diferenças entre o estado atual e a última versão.
Funciona de forma semelhante ao `git status`.

---

### ⏪ Restauração de versões

Permite voltar para versões anteriores com segurança.

---

### 🗑️ Remoção de versões

Exclui commits antigos quando não são mais necessários.

---

### 🛡️ Proteção contra perda de arquivos

Arquivos recém-criados ou não salvos **não são sobrescritos** ao trocar de versão.

---

### 🌍 Compatibilidade multiplataforma

* Windows
* Linux
* macOS

---

## 🛠️ Tecnologias Utilizadas

* .NET (C#)
* System.IO (manipulação de arquivos e diretórios)
* Arquitetura modular para evolução futura
* CLI (Command Line Interface)

---

## 🧠 Aprendizados com o Projeto

* Versionamento de arquivos na prática
* Manipulação de paths multiplataforma
* Segurança em operações destrutivas
* Design de ferramentas CLI
* Pensamento arquitetural inspirado em sistemas reais (como o Git)

---

## 💡 Resumo

O DeltaBox foca em **simplicidade, segurança e produtividade**, oferecendo uma alternativa leve para versionamento local sem a complexidade de ferramentas tradicionais.

