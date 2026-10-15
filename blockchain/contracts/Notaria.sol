// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

contract Notaria {
    struct Registro { address emisor; uint256 fecha; }

    mapping(bytes32 => Registro) private registros;

    event DocumentoRegistrado(bytes32 indexed hash, address indexed emisor, uint256 fecha);

    function registrar(bytes32 hash) external {
        require(registros[hash].fecha == 0, "Ya registrado");
        registros[hash] = Registro(msg.sender, block.timestamp);
        emit DocumentoRegistrado(hash, msg.sender, block.timestamp);
    }

    function verificar(bytes32 hash) external view returns (bool existe, address emisor, uint256 fecha) {
        Registro memory r = registros[hash];
        return (r.fecha != 0, r.emisor, r.fecha);
    }
}