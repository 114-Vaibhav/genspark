export class ProductCardModel{
    // title
    // price, description, thumbnail(image)
    constructor(public title: string, public price: number, public description: string, public thumbnail: string){
        
        this.title = title;
        this.price = price;
        this.description = description;
        this.thumbnail = thumbnail;
    }
}