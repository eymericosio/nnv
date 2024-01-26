import axios from "axios";
import * as config from "../config.json";

export default class MoviesApi {
	public list = async (): Promise<Array<IMovie>> => {
		var response = await axios.get<Array<IMovie>>(config.ApiRootUrl + "/movies");
		return response.data;
	};
}

export interface IMovie {
	key: string;
	name: string;
	description: string;
	rate: number;
	length: string;
	img: string;
	genres: Array<string>;
}
