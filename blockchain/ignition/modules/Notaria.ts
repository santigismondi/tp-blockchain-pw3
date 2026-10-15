import { buildModule } from "@nomicfoundation/hardhat-ignition/modules";

export default buildModule("NotariaModule", (m) => {
    const notaria = m.contract("Notaria");
    return { notaria };
});