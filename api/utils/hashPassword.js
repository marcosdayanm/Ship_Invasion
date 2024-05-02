import bcrypt from "bcrypt";

const hashPassword = async (password) => {
    const salt = await bcrypt.genSalt(10);
    return await bcrypt.hash(password, salt);
}

const comparePassword = async (password, hashPwd) => {
    return bcrypt.compareSync(password, hashPwd);
}

export {
    hashPassword,
    comparePassword
}