import React from 'react';
import ClienteForm from './components/ClienteForm';
import ConsultaRelatorio from './components/ConsultaRelatorio';
import './styles.css';  // Importando o arquivo CSS

function App() {
    return (
        <div className="container">
            <h1>Gerenciamento de Clientes</h1>
            <ClienteForm />
            <ConsultaRelatorio />
        </div>
    );
}

export default App;
