import React, { useState } from 'react';
import axios from 'axios';

const ConsultaRelatorio = () => {
    const [data, setData] = useState('');
    const [relatorio, setRelatorio] = useState(null);

    const handleChange = (e) => {
        setData(e.target.value);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.get(`https://localhost:7080/api/Consolidacao/consolidacao?data=${data}`);
            setRelatorio(response.data);
        } catch (error) {
            alert('Erro ao consultar relatório');
        }
    };

    return (
        <div>
            <h2>Consulta Relatório Diário</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Data: </label>
                    <input type="date" value={data} onChange={handleChange} />
                </div>
                <button type="submit">Consultar</button>
            </form>

            {relatorio && (
                <div className="relatorio">
                    <h3>Relatório do Dia {relatorio.data}</h3>
                    <p>Saldo Total: {relatorio.saldoTotal}</p>
                    <h4>Lançamentos:</h4>
                    <ul>
                        {relatorio.lancamentos?.map((lancamento) => (
                            <li key={lancamento.id}>
                                <span>{lancamento.tipopessoa}</span> - {lancamento.valor} - {lancamento.nome} ({lancamento.datacadastro})
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
};

export default ConsultaRelatorio;
