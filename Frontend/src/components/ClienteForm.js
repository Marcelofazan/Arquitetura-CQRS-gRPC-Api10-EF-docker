import React, { useState } from 'react';
import axios from 'axios';

const ClienteForm = () => {
    const [cliente, setCliente] = useState({
        nome: '',
        tipoPessoa: '',
        cpfCnpj: '',
        endereco: '',
        valor: 0,
        dataCadastro: ''
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setCliente((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {

            console.log(cliente);
            const response = await axios.post('https://localhost:7274/api/Cliente/create', cliente);
            alert('Registrado com sucesso!');
        } catch (error) {
            alert('Erro ao registrar');
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Formulário</h2>
            <div>
                <label>Tipo: </label>
                <select name="tipoPessoa" value={cliente.tipoPessoa} onChange={handleChange}>
                    <option value="">Selecione</option>
                    <option value="fisica">FÍSICA</option>
                    <option value="juridica">JURÍDICA</option>
                </select>
            </div>
            <div>
                <label>Nome: </label>
                <input type="text" name="nome" value={cliente.nome} onChange={handleChange} />
            </div>
            <div>
                <label>CPF/CNPJ: </label>
                <input type="text" name="cpfCnpj" value={cliente.cpfCnpj} onChange={handleChange} />
            </div>
            <div>
                <label>Endereço: </label>
                <input type="text" name="endereco" value={cliente.endereco} onChange={handleChange} />
            </div>
            <div>
                <label>Valor: </label>
                <input type="number" name="valor" value={cliente.valor} onChange={handleChange} />
            </div>
            <div>
                <label>Data: </label>
                <input type="date" name="dataCadastro" value={cliente.dataCadastro} onChange={handleChange} />
            </div>
            <button type="submit">Salvar</button>
        </form>
    );
};

export default ClienteForm;
